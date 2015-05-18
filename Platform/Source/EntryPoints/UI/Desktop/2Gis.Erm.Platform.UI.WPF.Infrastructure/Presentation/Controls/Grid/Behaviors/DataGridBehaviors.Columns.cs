using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Behaviors
{
    /// <summary>
    /// Используется для грида типа DataGrid
    /// </summary>
    public static partial class DataGridBehaviors
    {
        #region ColumnsProperty

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(IEnumerable<GridColumnViewModel>), typeof(DataGridBehaviors), new PropertyMetadata(RegenerateGridColumns));

        public static void SetColumns(UIElement element, GridColumnViewModel value)
        {
            element.SetValue(ColumnsProperty, value);
        }

        public static GridColumnViewModel GetColumns(UIElement element)
        {
            return (GridColumnViewModel)element.GetValue(ColumnsProperty);
        }

        #endregion

        private static void RegenerateGridColumns(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var collection = e.NewValue as IEnumerable<GridColumnViewModel>;
            if (collection == null)
            {
                return;
            }

            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            dataGrid.Columns.Clear();
            foreach (var column in collection)
            {
                dataGrid.Columns.Add(CreateColumn(column, dataGrid.DataContext));
            }
        }

        private static DataGridColumn CreateColumn(GridColumnViewModel column, object gridDataContext)
        {
            if (!string.IsNullOrEmpty(column.ReferenceTo))
            {

                var hyperlinkColumn = new DataGridTemplateColumn
                    {
                        Header = column.LocalizedName,
                        Width = column.Width,
                        Visibility = column.Width == 0 || column.Hidden ? Visibility.Collapsed : Visibility.Visible,
                        CellTemplate = CreateHyperlinkTemplate(column, gridDataContext),
                        SortMemberPath = column.Name
                    };

                return hyperlinkColumn;
            }

            switch (column.Type)
            {
                default:
                    return new DataGridTextColumn
                    {
                        /// FIXME {all, 03.03.2014}: по запросу Максима (задача ERM-3203) был применен shelveset (ERM-3203 Core от 3.3.2014) до этого было Binding = new Binding(column.Name) { StringFormat = column.DotNetType.Contains("Date")  ? "dd.MM.yyyy" : null}, Максим обещал доработать 
                        Binding = new Binding(column.Name) { StringFormat = null},
                        Header = column.LocalizedName,
                        Width = column.Hidden ? 0 : column.Width,
                        Visibility = column.Width == 0 || column.Hidden ? Visibility.Collapsed : Visibility.Visible,
                        SortMemberPath = column.Name
                    };
            }
        }

        /// <summary>
        /// Динамически создает темплейт для поля типа "ссылка".
        /// На выходе получается что-то подобное:
        /// <DataTemplate>
        ///     <TextBlock>
        ///         <Hyperlink Command="{Binding NavigateCommand}">
        ///             <Hyperlink.CommandParameter>
        ///                 <MultiBinding>
        ///                     <Binding Path="$columnViewModel.ReferenceKeyField"/>
        ///                     <Binding Source="$columnViewModel.ReferenceTo"/>
        ///                 </MultiBinding>
        ///             </Hyperlink.CommandParameter>
        ///             <TextBlock Text="{Binding $column.Name}"/>
        ///         </Hyperlink>
        ///     </TextBlock>
        /// </DataTemplate>
        /// </summary>
        private static DataTemplate CreateHyperlinkTemplate(GridColumnViewModel columnViewModel, object gridDataContext)
        {
            var hyperlinkTemplate = new DataTemplate();

            var outerTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock)) { Name = "factory" };

            var nestedTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            nestedTextBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(columnViewModel.Name));

            var hyperlinkFactory = new FrameworkElementFactory(typeof(Hyperlink));
            hyperlinkFactory.SetBinding(Hyperlink.CommandProperty, new Binding("NavigateCommand") { Source = gridDataContext });

            // Команде нужно два параметра - имя сущности и ее идентификатор. 
            // Для преобразования их в один (NavigationDescriptor) используется NavigationParameterConverter
            hyperlinkFactory.SetBinding(Hyperlink.CommandParameterProperty,
                                        new MultiBinding
                                            {
                                                Bindings =
                                                    {
                                                        // Поле с Id сущности
                                                        new Binding(columnViewModel.ReferenceKeyField),
                                                        // Имя сущности
                                                        new Binding { Source = columnViewModel.ReferenceTo }
                                                    },
                                                Converter = new NavigationParameterConverter()
                                            });

            hyperlinkFactory.AppendChild(nestedTextBlockFactory);
            outerTextBlockFactory.AppendChild(hyperlinkFactory);

            hyperlinkTemplate.VisualTree = outerTextBlockFactory;
            return hyperlinkTemplate;
        }

        private class NavigationParameterConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }

                var entityId = -1L;
                IEntityType entityName = EntityType.Instance.None();

                try
                {
                    entityId = System.Convert.ToInt64(values[0]);
                    EntityType.Instance.TryParse(System.Convert.ToString(values[1]), out entityName);
                }
                catch (FormatException)
                {
                }
                catch (InvalidCastException)
                {
                }

                return new NavigationDescriptor
                    {
                        EntityName = entityName,
                        EntityId = entityId
                    };
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}