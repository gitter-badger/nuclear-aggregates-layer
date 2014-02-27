using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace DoubleGis.OLAP.Reports
{
	/// <summary>
	/// Значения параметров и строк подключения по умолчанию
	/// В основном для удобства разработки и тестирования
	/// </summary>
	internal class Defaults
	{
		public static readonly int CITY = 0;
		public static readonly int CURRENT_USER = 0;
		public static readonly DateTime ISSUEDATE = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);

		public static readonly SqlConnection WAREHOUSE_CONNECTION =
			new SqlConnection("Data Source=localhost;Initial Catalog=WH1;Integrated Security=True;");

		public static readonly SqlConnection ERM_CONNECTION =
			new SqlConnection("Data Source=uk-erm-test.2gis.local;Initial Catalog=Erm04;Integrated Security=False;User ID=sa;Password=123qwe!");
	}
}
