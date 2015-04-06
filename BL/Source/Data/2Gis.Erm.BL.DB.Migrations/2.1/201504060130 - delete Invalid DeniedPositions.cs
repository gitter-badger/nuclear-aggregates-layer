﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201504060130, "Удаляем невалидные правила запрещения", "y.baranihin")]
    public class Migration201504060130 : TransactedMigration
    {
        private const string ValidationQuery = @"select count(*)
  FROM [Billing].[DeniedPositions] dp 
  left join [Billing].[DeniedPositions] dp1 on dp.PriceId = dp1.PriceId and dp.PositionId = dp1.PositionDeniedId and dp.PositionDeniedId = dp1.PositionId
  where dp.PositionId != dp.PositionDeniedId and dp.IsDeleted = 0 and dp1.Id is null";

        #region Записи к удалению
        private readonly long[] _deniedPositionsToDelete =
            {
                73292,
                91390,
                91464,
                91472,
                91473,
                91480,
                91481,
                127236,
                127244,
                127245,
                127249,
                127250,
                141582,
                177326,
                202324,
                229693,
                429087,
                642222,
                855112,
                948797,
                1093839,
                1236990,
                1295902,
                1361020,
                1377265,
                1514311,
                1514325,
                1514341,
                1514359,
                1514379,
                1514401,
                1514425,
                1514451,
                1514479,
                1514509,
                1530543,
                1530939,
                1559030,
                1567722,
                1571097,
                1589246,
                1589254,
                1589268,
                1589284,
                1589302,
                1589322,
                1589344,
                1589368,
                1589394,
                1589422,
                1589452,
                1589653,
                1593064,
                1599519,
                1602977,
                1602991,
                1603007,
                1603026,
                1603045,
                1603068,
                1603091,
                1603118,
                1603145,
                1603176,
                1644364,
                1802313,
                1802328,
                1802344,
                1802361,
                1802381,
                1802404,
                1802428,
                1802453,
                1802481,
                1802512,
                1812805,
                1840541,
                1911070,
                1916001,
                1929922,
                1940352,
                1947358,
                1947363,
                1947364,
                1947365,
                1947366,
                1947367,
                1947368,
                1947369,
                1947370,
                1947371,
                1947372,
                1954263,
                1961104,
                2026328,
                2026330,
                2026331,
                2026332,
                2026333,
                2026334,
                2026335,
                2026336,
                2026337,
                2026338,
                2026339,
                2029804,
                2058188,
                2058191,
                2058192,
                2058193,
                2058194,
                2058195,
                2058196,
                2058197,
                2058198,
                2058199,
                2058200,
                2093689,
                2165770,
                2169556,
                2194294,
                2204567,
                2218418,
                2218435,
                2218452,
                2218513,
                2218530,
                2218548,
                2218565,
                2218582,
                2218599,
                2218616,
                2218633,
                2225323,
                2290544,
                2290561,
                2290578,
                2290636,
                2290653,
                2290671,
                2290688,
                2290705,
                2290722,
                2290739,
                2290756,
                2301802,
                2332728,
                2332745,
                2332762,
                2332816,
                2332833,
                2332851,
                2332868,
                2332885,
                2332902,
                2332919,
                2332936,
                2340420,
                2364565,
                2463958,
                2463975,
                2463992,
                2464050,
                2464067,
                2464089,
                2464106,
                2464123,
                2464140,
                2464157,
                2464174,
                2467443,
                2499249,
                2512957,
                2512974,
                2512991,
                2513045,
                2513062,
                2513084,
                2513101,
                2513118,
                2513135,
                2513152,
                2513169,
                2547754,
                2582983,
                2583000,
                2583017,
                2583078,
                2583095,
                2583113,
                2583130,
                2583147,
                2583164,
                2583181,
                2583198,
                2590066,
                2597186
            }; 
        #endregion   

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("Update [Billing].[DeniedPositions] set IsActive = 0, IsDeleted = 1, ModifiedOn = Getutcdate(), ModifiedBy = 1 where Id in ({0})",
                                                             string.Join(",", _deniedPositionsToDelete.Select(x => x.ToString()))));

            var notDeleted = (int)context.Connection.ExecuteScalar(ValidationQuery);
            if (notDeleted > 0)
            {
                throw new Exception(string.Format("Появилось {0} новых невалидных правил запрещения.", notDeleted));
            }
        }
    }
}