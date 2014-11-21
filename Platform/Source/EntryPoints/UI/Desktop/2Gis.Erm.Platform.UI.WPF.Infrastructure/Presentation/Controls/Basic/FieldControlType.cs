namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    /// <summary>
    /// Тип контрола для редактирования/отображения значения в FieldEditControl.
    /// </summary>
    public enum FieldControlType
    {
        /// <summary>
        /// Шаблон автоматически определяется исходя из типа свойства, к которому привязан контрол.
        /// </summary>
        Auto,

        /// <summary>
        /// Шаблон указан явно в свойстве CustomContentTemplate.
        /// </summary>
        Custom,
        ReadOnlyText,
        TextBox,
        DateTime,
        CheckBox,
        YesNoRadio,
        EnumComboBox
    }
}
