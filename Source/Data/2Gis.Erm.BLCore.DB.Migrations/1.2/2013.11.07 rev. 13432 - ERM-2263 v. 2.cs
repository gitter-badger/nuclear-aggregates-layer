using System.Linq;
using System.Text;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13432, "Настройка прав для пользователя ЛК")]
    public sealed class Migration13432 : TransactedMigration
    {
        private const string RoleName = "Пользователь личного кабинета";
        private const string UserName = "LKERMUser";
        private const int ReadOperationId = 1;
        private const int EditOperationId = 2;
        private const int CreateOperationId = 32;
        private const int AdvertisementCode = 186;
        private const int AdvertisementElementCode = 187;
        private const int OrderProcessingRequestCode = 550;

        private const long LkRoleId = 224824852375216903;
        private const long UserRoleId = 224826130908128263;
        

        #region Набор идентификаторов

        private readonly long[] _pregeneratedIds =
            {
                224828667690296583,
                224828667950343687,
                224828668218779399,
                224828668478826503,
                224828668738873607,
                224828669133138439,
                224828669787450119,
                224828670047497223,
                224828670307544327,
                224828670701809159,
                224828670836027143,
                224828671490338823,
                224828671876215047,
                224828672404697607,
                224828672798962439,
                224828673059009543,
                224828673713321223,
                224828674233415175,
                224828674761897735,
                224828675416209415,
                224828675676256519,
                224828675936303623,
                224828676196350727,
                224828676330568711,
                224828676590615815,
                224828676850662919,
                224828677110710023,
                224828677379145735,
                224828677639192839,
                224828677899239943,
                224828678033457927,
                224828678159287303,
                224828678419334407,
                224828678553552391,
                224828678813599495,
                224828679073646599,
                224828679207864583,
                224828679342082567,
                224828679467911943,
                224828679727959047,
                224828679862177031,
                224828680122224135,
                224828680516488967,
                224828680650706951,
                224828680910754055,
                224828681036583431,
                224828681170801415,
                224828681430848519,
                224828681565066503,
                224828681690895879,
                224828681959331591,
                224828682085160967,
                224828682219378951,
                224828682345208327,
                224828682613644039,
                224828682739473415,
                224828682999520519,
                224828683133738503,
                224828683267956487,
                224828683528003591,
                224828683653832967,
                224828683788050951,
                224828683922268935,
                224828684308145159,
                224828684442363143,
                224828684576581127,
                224828684702410503,
                224828684962457607,
                224828685096675591,
                224828685356722695,
                224828685490940679,
                224828685750987783,
                224828686145252615,
                224828686539517447,
                224828686799564551,
                224828687059611655,
                224828687319658759,
                224828688502452743,
                224828689022546695,
                224828689542640647,
                224828690196952327,
                224828690591217159,
                224828690985481991,
                224828691379746823,
                224828691639793927,
                224828692159887879,
                224828692688370439,
                224828693342682119,
                224828693862776071,
                224828694257040903,
                224828694517088007,
                224828695171399687,
                224828695565664519,
                224828696085758471,
                224828696354194183,
                224828696614241287,
                224828696874288391,
                224828697134335495,
                224828697662818055,
                224828698442958855,
                224828699097270535,
                224828700665940487,
                224828701060205319,
                224828701454470151,
                224828701848734983,
                224828702108782087,
                224828702503046919,
                224828702897311751,
                224828703551623431,
                224828703937499655,
                224828704860246791,
                224828705246123015,
                224828705514558727,
                224828706034652679,
                224828706428917511,
                224828706823182343,
                224828707603323143,
                224828707863370247,
                224828708131805959,
                224828708517682183,
                224828708786117895,
                224828709046164999,
                224828709306212103,
                224828709566259207,
                224828709960524039,
                224828710220571143,
                224828710480618247,
                224828710749053959,
                224828711134930183,
                224828711269148167,
                224828711529195271,
                224828711663413255,
                224828711923460359,
                224828712057678343,
                224828713752177415,
                224828713886395399,
                224828714146442503,
                224828714280660487,
                224828714540707591,
                224828714800754695,
                224828715060801799,
                224828715329237511,
                224828715849331463,
                224828716109378567,
                224828716503643399,
                224828717292172807,
                224828717552219911,
                224828717678049287,
                224828717946484999,
                224828718072314375,
                224828718332361479,
                224828718466579463,
                224828718600797447,
                224828718726626823,
                224828719775203079,
                224828719909421063,
                224828720169468167,
                224828720295297543,
                224828720429515527,
                224828720689562631,
                224828720823780615,
                224828721218045447,
                224828721478092551,
                224828721738139655,
                224828721998186759,
                224828722392451591,
                224828722786716423,
                224828723306810375,
                224828723566857479,
                224828723835293191,
                224828724095340295,
                224828724489605127,
                224828725009699079,
                224828725403963911,
                224828725664011015,
                224828725924058119,
                224828726184105223,
                224828726452540935,
                224828726838417159,
                224828727232681991,
                224828728801351943,
                224828729069787655,
                224828729329834759,
                224828729724099591,
                224828729984146695,
                224828730504240647,
                224828730764287751,
                224828731032723463,
                224828731418599687,
                224828731687035399,
                224828731947082503,
                224828732207129607,
                224828732467176711,
                224828732727223815,
                224828733381535495,
                224828733649971207,
                224828733910018311,
                224828734170065415,
                224828734430112519,
                224828734690159623,
                224828734958595335,
                224828735218642439,
                224828735612907271,
                224828735872954375,
                224828736133001479,
                224828736393048583,
                224828736787313415,
                224828737047360519,
                224828737441625351,
                224828737701672455,
                224828738355984135,
                224828738884466695,
                224828739144513799,
                224828739404560903,
                224828739798825735,
                224828740058872839,
                224828740318919943,
                224828740578967047,
                224828740973231879,
                224828741107449863,
                224828741367496967,
                224828741501714951,
                224828742021808903,
                224828742156026887,
                224828742810338567,
                224828742936167943,
                224828743070385927,
                224828743196215303,
                224828743330433287,
                224828743464651271,
                224828743590480647,
                224828743850527751,
                224828743984745735,
                224828744118963719,
                224828744379010823,
                224828744504840199,
                224828744639058183,
                224828744899105287,
                224828745159152391,
                224828745293370375,
                224828745427588359,
                224828745687635463,
                224828745813464839,
                224828745947682823,
                224828746081900807,
                224828746341947911,
                224828746467777287,
                224828746601995271,
                224828746862042375,
                224828747256307207,
                224828747390525191,
                224828747516354567,
                224828747776401671,
                224828747910619655,
                224828748044837639,
                224828748304884743,
                224828748430714119,
                224828748564932103,
                224828748824979207,
                224828748959197191,
                224828749085026567,
                224828749219244551,
                224828749353462535,
                224828749479291911,
                224828749739339015,
                224828749873556999,
                224828750007774983,
                224828750267822087,
                224828750393651463,
                224828750662087175,
                224828750787916551,
                224828750922134535,
                224828751182181639,
                224828751316399623,
                224828751442228999,
                224828751576446983,
                224828751836494087,
                224828751970712071,
                224828752096541447,
                224828752356588551,
                224828752490806535,
                224828752625024519,
                224828752885071623,
                224828753405165575,
                224828754193694983,
                224828754587959815,
                224828754848006919,
                224828755368100871,
                224828755502318855,
                224828755628148231,
                224828755896583943,
                224828756022413319,
                224828756156631303,
                224828756416678407,
                224828756550896391,
                224828756810943495,
                224828756936772871,
                224828757070990855,
                224828757205208839,
                224828757465255943
            };

        #endregion

        private const string PrepareIdsQuery = "INSERT INTO @Ids Values ";
        

        private string _query = @"
Declare @LkRoleId bigint
Declare @LkUserId bigint

if exists(SELECT * FROM [Security].[Roles] WHERE Name = '{0}') AND exists(SELECT * FROM Security.Users Where Account = '{1}')
BEGIN
    SET @LkRoleId = (SELECT TOP 1 Id FROM Security.Roles Where Name = '{0}')
    SET @LkUserId = (SELECT TOP 1 Id FROM Security.Users Where Account = '{1}')

    DELETE FROM [Security].[UserRoles] WHERE UserId = @LkUserId AND RoleId = @LkRoleId
    DELETE FROM [Security].[RolePrivileges] WHERE RoleId = @LkRoleId
    DELETE FROM [Security].[Roles] WHERE Name = '{0}'
END   

If not exists(SELECT * FROM [Security].[Roles] WHERE Name = '{0}')
  INSERT INTO [Security].[Roles] (Id, Name, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn) VALUES ({8}, '{0}', 1, 1, GETUTCDATE(), GETUTCDATE())

SET @LkRoleId = (SELECT TOP 1 Id FROM Security.Roles Where Name = '{0}')
SET @LkUserId = (SELECT TOP 1 Id FROM Security.Users Where Account = '{1}')

If not exists(SELECT * FROM [Security].[UserRoles] WHERE UserId = @LkUserId AND RoleId = @LkRoleId)
  INSERT INTO [Security].[UserRoles] (Id, UserId, RoleId, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn) VALUES ({9}, @LkUserId, @LkRoleId, 1, 1, GETUTCDATE(), GETUTCDATE())

 DELETE FROM [Security].[RolePrivileges] WHERE RoleId = @LkRoleId

Declare @Ids Shared.Int64IdsTableType

{10}

DECLARE @PrivilegeId bigint
DECLARE @currentId bigint
Declare @PrivilegeIds Shared.Int64IdsTableType

INSERT INTO @PrivilegeIds SELECT Id FROM [Security].[Privileges] where Operation = {2}
SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds)
WHILE @PrivilegeId IS NOT NULL
BEGIN
    SET @currentId  = (SELECT min(Id) FROM @Ids)
     INSERT INTO [Security].[RolePrivileges] (Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
        VALUES (@currentId, @LkRoleId, @PrivilegeId, 0, 16, 1, 1, getutcdate(), getutcdate())
    
    DELETE FROM @Ids WHERE Id = @currentId
    SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds WHERE Id > @PrivilegeId)
END

DELETE FROM @PrivilegeIds
INSERT INTO @PrivilegeIds SELECT Id FROM [Security].[Privileges] where Operation = {3} AND EntityType = {5}
SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds)
WHILE @PrivilegeId IS NOT NULL
BEGIN
    SET @currentId  = (SELECT min(Id) FROM @Ids)
     INSERT INTO [Security].[RolePrivileges] (Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
        VALUES (@currentId, @LkRoleId, @PrivilegeId, 0, 16, 1, 1, getutcdate(), getutcdate())
    
    DELETE FROM @Ids WHERE Id = @currentId
    SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds WHERE Id > @PrivilegeId)
END

DELETE FROM @PrivilegeIds
INSERT INTO @PrivilegeIds SELECT Id FROM [Security].[Privileges] where Operation = {3} AND EntityType = {6}
SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds)
WHILE @PrivilegeId IS NOT NULL
BEGIN
    SET @currentId  = (SELECT min(Id) FROM @Ids)
     INSERT INTO [Security].[RolePrivileges] (Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
        VALUES (@currentId, @LkRoleId, @PrivilegeId, 0, 16, 1, 1, getutcdate(), getutcdate())
    
    DELETE FROM @Ids WHERE Id = @currentId
    SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds WHERE Id > @PrivilegeId)
END

DELETE FROM @PrivilegeIds
INSERT INTO @PrivilegeIds SELECT Id FROM [Security].[Privileges] where Operation = {4} AND EntityType = {7}
SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds)
WHILE @PrivilegeId IS NOT NULL
BEGIN
    SET @currentId  = (SELECT min(Id) FROM @Ids)
     INSERT INTO [Security].[RolePrivileges] (Id, RoleId, PrivilegeId, Priority, Mask, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
        VALUES (@currentId, @LkRoleId, @PrivilegeId, 0, 16, 1, 1, getutcdate(), getutcdate())
    
    DELETE FROM @Ids WHERE Id = @currentId
    SET @PrivilegeId = (SELECT min(Id) FROM @PrivilegeIds WHERE Id > @PrivilegeId)
END";

        protected override void ApplyOverride(IMigrationContext context)
        {
            EnsureLkUser(context);

            var idsBuilder = new StringBuilder(PrepareIdsQuery);
            for (var i = 0; i < _pregeneratedIds.Count() - 1; i++)
            {
                idsBuilder.AppendLine(string.Format("({0}),", _pregeneratedIds[i]));
            }
            idsBuilder.AppendLine(string.Format("({0})", _pregeneratedIds.Last()));

            _query = string.Format(_query,
                                  RoleName,
                                  UserName,
                                  ReadOperationId,
                                  EditOperationId,
                                  CreateOperationId,
                                  AdvertisementCode,
                                  AdvertisementElementCode,
                                  OrderProcessingRequestCode,
                                  LkRoleId,
                                  UserRoleId,
                                  idsBuilder);

            
            context.Database.ExecuteNonQuery(_query);
        }

        private void EnsureLkUser(IMigrationContext context)
        {
            #region Набор идентификаторов

            long[] pregeneratedIds =
                {
                    225564364986473479,
                    225564376764079367,
                    225564380429901319,
                    225564380689948423,
                    225564380949995527,
                    225564381344260359,
                    225564381604307463,
                    225564381872743175,
                    225564381998572551,
                    225564384489989383,
                    225564385404347911,
                    225564385530177287,
                    225564385798612999,
                    225564386058660103,
                    225564386318707207,
                    225564386452925191,
                    225564389590264839,
                    225564389850311943,
                    225564389984529927,
                    225564390244577031,
                    225564390504624135,
                    225564390638842119,
                    225564390764671495,
                    225564392467559175,
                    225564392601777159,
                    225564392861824263,
                    225564393121871367,
                    225564393256089351,
                    225564393650354183,
                    225564393910401287,
                    225564394036230663,
                    225564394304666375,
                    225564394564713479,
                    225564394690542855,
                    225564394958978567,
                    225564395084807943,
                    225564395344855047,
                    225564395613290759,
                    225564395873337863,
                    225564395999167239,
                    225564396267602951,
                    225564396527650055,
                    225564397047744007,
                    225564398490584839,
                    225564399010678791,
                    225564399270725895,
                    225564399664990727,
                    225564400193473287,
                    225564401107831815,
                    225564401502096647,
                    225564402282237447,
                    225564402676502279,
                    225564403070767111,
                    225564403590861063,
                    225564404379390471,
                    225564404899484423,
                    225564405159531527,
                    225564405553796359,
                    225564405948061191,
                    225564406208108295,
                    225564406468155399,
                    225564406736591111,
                    225564406996638215,
                    225564407122467591,
                    225564407256685575,
                    225564407516732679,
                    225564407910997511,
                    225564408565309191,
                    225564409085403143,
                    225564409353838855,
                    225564409739715079,
                    225564410133979911,
                    225564410394027015,
                    225564410922509575,
                    225564422960162311,
                    225564432514787079,
                    225564437355014151,
                    225564444946704647,
                    225564453058488839,
                    225564459341556487,
                    225564469676321799,
                    225564478836681991,
                    225564479365164551,
                    225564479759429383,
                    225564486428372999,
                    225564496377262343,
                    225564509329273351,
                    225564510377849607,
                    225564510637896711,
                    225564510897943815,
                    225564511032161799,
                    225564511166379783,
                    225564511292209159,
                    225564511426427143,
                    225564511552256519,
                    225564511820692231,
                    225564511946521607,
                    225564512080739591,
                    225564512206568967,
                    225564512735051527,
                    225564513783627783,
                    225564514303721735,
                    225564514563768839,
                    225564514958033671,
                    225564515218080775,
                    225564515478127879,
                    225564515746563591,
                    225564516006610695,
                    225564519403997191,
                    225564519538215175,
                    225564519798262279,
                    225564519932480263,
                    225564520058309639,
                    225564520326745351,
                    225564522809773575,
                    225564523329867527,
                    225564523598303239,
                    225564523724132615,
                    225564523984179719,
                    225564525292802823,
                    225564525427020807,
                    225564525561238791,
                    225564526081332743,
                    225564526215550727,
                    225564526341380103,
                    225564526475598087,
                    225564526601427463,
                    225564526869863175,
                    225564526995692551,
                    225564527389957383,
                    225564527524175367,
                    225564527650004743,
                    225564527910051847,
                    225564528178487559,
                    225564528304316935,
                    225564528438534919,
                    225564528564364295,
                    225564528832800007,
                    225564528958629383,
                    225564529092847367,
                    225564529218676743,
                    225564529487112455,
                    225564529612941831,
                    225564529747159815,
                    225564530141424647,
                    225564530401471751,
                    225564530527301127,
                    225564532624453383,
                    225564533538811911,
                    225564533933076743,
                    225564534193123847,
                    225564534327341831,
                    225564534453171207,
                    225564534721606919,
                    225564534981654023,
                    225564535107483399,
                    225564535375919111,
                    225564535501748487,
                    225564535761795591,
                    225564535896013575,
                    225564536944589831,
                    225564538647477511,
                    225564539033353735,
                    225564539167571719,
                    225564539427618823,
                    225564539561836807,
                    225564539821883911,
                    225564540081931015,
                    225564540341978119,
                    225564540476196103,
                    225564540736243207,
                    225564540870461191,
                    225564541130508295,
                    225564541264726279,
                    225564541524773383,
                    225564541650602759,
                    225564541919038471,
                    225564542044867847,
                    225564542304914951,
                    225564542439132935,
                    225564542699180039,
                    225564543881974023,
                    225564545056379399,
                    225564545190597383,
                    225564545970738183,
                    225564546499220743,
                    225564546885096967,
                    225564547153532679,
                    225564547413579783,
                    225564547933673735,
                    225564548462156295,
                    225564548587985671,
                    225564548848032775,
                    225564548982250759,
                    225564549242297863,
                    225564549896609543,
                    225564550030827527,
                    225564550425092359,
                    225564550550921735,
                    225564550810968839
                };

        #endregion

            // взал с боевой базы России
            const long LkUserId = 3294;

            var ensureLkUserQuery = @"
                If not exists(SELECT * FROM Security.Users Where Account = '{0}')
                BEGIN
                    INSERT INTO Security.Users (Id, Account, FirstName, LastName, DisplayName, DepartmentId, ParentId, IsDeleted, IsActive, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, IsServiceUser) 
                        VALUES ({1}, '{0}', 'Кабинета', 'Пользователь', 'Пользователь, Кабинета', 1, NULL, 0, 1, 1, 1, GETUTCDATE(), GETUTCDATE(), 0)

                        DECLARE @OrganizationUnitId bigint
                        DECLARE @CurrentId bigint
                        DECLARE @OrganizationUnitIds Shared.Int64IdsTableType

                        DECLARE @Ids Shared.Int64IdsTableType

                        {2}

                        INSERT INTO @OrganizationUnitIds SELECT Id FROM [Billing].[OrganizationUnits] where IsActive = 1
                        SET @OrganizationUnitId= (SELECT min(Id) FROM @OrganizationUnitIds)
                        WHILE @OrganizationUnitId IS NOT NULL
                        BEGIN
                            SET @CurrentId  = (SELECT min(Id) FROM @Ids)
                            INSERT INTO Security.UserOrganizationUnits (Id, UserId, OrganizationUnitId, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn)
                                VALUES (@CurrentId, {1}, @OrganizationUnitId, 1, 1, getutcdate(), getutcdate())
    
                            DELETE FROM @Ids WHERE Id = @CurrentId
                            SET @OrganizationUnitId = (SELECT min(Id) FROM @OrganizationUnitIds WHERE Id > @OrganizationUnitId)
                        END
                END";

            var prepareIdsQuery = "INSERT INTO @Ids Values " + string.Join(", ", pregeneratedIds.Select(x => string.Format("({0})", x)));

            ensureLkUserQuery = string.Format(ensureLkUserQuery, UserName, LkUserId, prepareIdsQuery);

            context.Database.ExecuteNonQuery(ensureLkUserQuery);
        }
    }
}
