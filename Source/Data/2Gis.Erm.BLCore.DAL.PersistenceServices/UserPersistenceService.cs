using System;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class UserPersistenceService : IUserPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public UserPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public int CheckUserParentnessRecursion(long userId, long parentId)
        {
            return _databaseCaller.ExecuteProcedureWithResultSingleValue<int>("Security.CheckUserParentnessRecursion",
                                                                        new Tuple<string, object>("userId", userId),
                                                                        new Tuple<string, object>("proposedParentId", parentId));
        }
    }
}