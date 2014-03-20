using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.BL.Print
{
    public static class PrintDataSpecs
    {
        [Tags("BL", "Print")]
        [Subject(typeof(PrintOrderHandler))]
        public class WhenConcatenatingSimpleData
        {
            private static PrintData PrintData1;
            private static PrintData PrintData2;
            private static PrintData PrintDataResult;
            
            Establish context = () =>
                {
                    PrintData1 = new PrintData { { "a", 1 } };
                    PrintData2 = new PrintData { { "b", 2 } };
                };

            Because of = () => { PrintDataResult = PrintData.Concat(PrintData1, PrintData2); };

            It should_not_be_null = () => PrintDataResult.Should().NotBeNull();
            It should_contain_data_from_first_object = () => PrintDataResult.GetData("a").Should().Be(1);
            It should_contain_data_from_second_object = () => PrintDataResult.GetData("b").Should().Be(2);
        }

        [Tags("BL", "Print")]
        [Subject(typeof(PrintOrderHandler))]
        public class WhenAddingComplexData
        {
            private static PrintData PrintData;
            private static PrintData PrintDataResult;

            Establish context = () =>
            {
                PrintData = new PrintData { { "a.b.c", 1 }, { "a.b.d", 1 } };
            };

            Because of = () => { };

            It should_contain_data_all_path_1 = () => PrintData.GetData("a").Should().NotBeNull();
            It should_contain_data_all_path_2 = () => PrintData.GetData("a.b").Should().NotBeNull();
            It should_contain_data_all_path_3 = () => PrintData.GetData("a.b.c").Should().NotBeNull();
            It should_contain_data_all_path_4 = () => PrintData.GetData("a.b.d").Should().NotBeNull();
            It should_be_containers_at_halfway_1 = () => PrintData.GetData("a").Should().BeOfType<PrintData>();
            It should_be_containers_at_halfway_2 = () => PrintData.GetData("a.b").Should().BeOfType<PrintData>();
        }

        [Tags("BL", "Print")]
        [Subject(typeof(PrintOrderHandler))]
        public class WhenConcatenatingComplexData
        {
            private static PrintData PrintData1;
            private static PrintData PrintData2;
            private static PrintData PrintDataResult;

            Establish context = () =>
            {
                PrintData1 = new PrintData { { "a.b.c", 1 } };
                PrintData2 = new PrintData { { "a.b.d", 2 } };
            };

            Because of = () => { PrintDataResult = PrintData.Concat(PrintData1, PrintData2); };

            It should_contain_data_all_path_1 = () => PrintDataResult.GetData("a.b.c").Should().Be(1);
            It should_contain_data_all_path_2 = () => PrintDataResult.GetData("a.b.d").Should().Be(2);
        }

        [Tags("BL", "Print")]
        [Subject(typeof(PrintOrderHandler))]
        public class WhenEnumeratingData
        {
            private static PrintData PrintData;

            Establish context = () =>
            {
                PrintData = new PrintData();
            };

            Because of_populating_with_data = () => 
            {
                PrintData.Add("a.b.c", 1);
                PrintData.Add("a.b.d.e", 2);
                PrintData.Add("a.b.f", new PrintData { { "g", 3 } });
                PrintData.Add("a.b.h", new[] { new PrintData { { "i", 4 } }, new PrintData { { "i", 5 } }, new PrintData { { "i", 6 } } });
            };

            It should_enumerate_through_all_values = () => PrintData.Select(pair => pair.Key).Should().Contain("a.b.c", "a.b.d.e", "a.b.f.g");
            It should_enumerate_through_all_collections = () => PrintData.Select(pair => pair.Key).Should().Contain("a.b.h");
            It should_not_enumerate_through_any_containers = () => PrintData.Select(pair => pair.Key).Should().NotContain("a", "a.b", "a.b.d", "a.b.f");
        }
    }
}
