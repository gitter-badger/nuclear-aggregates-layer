using System.Text;

namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public static class SqlHelper
    {
        public static StringBuilder AppendSqlDropConstraint(this StringBuilder sb, SchemaQualifiedObjectName tableName, string constraintName)
        {
            return sb.AppendFormat("ALTER TABLE {0} DROP CONSTRAINT {1};\n", tableName, constraintName);
        }

        /// <summary>
        /// Генерит SQL для добавления ограничения на неравенство значения колонки поля пустой строке 
        /// (с предварительным обновлением данных).
        /// </summary>
        public static StringBuilder AppendNotEmptyConstraintWithDataUpdate(this StringBuilder sb, 
            SchemaQualifiedObjectName tableName, 
            string columnName, 
            string constraintName = null)
        {
            if (constraintName == null)
            {
                constraintName = string.Format("CK_{0}_Not_Empty", columnName);
            }

            sb.AppendFormat("UPDATE {0} SET {1} = NULL WHERE {1} = '';\n" + "ALTER TABLE {0} ADD CONSTRAINT {2} CHECK ({1} <> '');\n",
                            tableName,
                            columnName, 
                            constraintName);
            return sb;
        }

        /// <summary>
        /// Генерит SQL для добавления ограничения на неравенство значения колонки поля пустой строке 
        /// (с предварительным обновлением данных).
        /// </summary>
        public static StringBuilder AppendNotNegaiveConstraintWithDataUpdate(this StringBuilder sb, 
            SchemaQualifiedObjectName tableName,
            string columnName, 
            string constraintName)
        {
            if (constraintName == null)
            {
                constraintName = string.Format("CK_{0}_Not_Negaive", columnName);
            }

            sb.AppendFormat("ALTER TABLE {0} ADD CONSTRAINT {2} CHECK ({1} >= 0);\n", tableName, columnName, constraintName);
            return sb;
        }
    }
}
