using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Import
{
    [Tags("BL")]
    [Subject(typeof(ImportFlowFinancialData1CHandler))]
    public class ImportFlowFinancialData1CHandlerSpecs
    {
        private static OperationType GetOperationType(long id, bool isPlus, int operation_type_code_1c)
        {
            return new OperationType
            {
                Id = id,
                IsPlus = isPlus,
                SyncCode1C = operation_type_code_1c.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static SimplifiedAccountDetail Simplify(AccountDetail accountDetail)
        {
            return new SimplifiedAccountDetail
            {
                Id = accountDetail.Id,
                AccountId = accountDetail.AccountId,
                TransactionDate = accountDetail.TransactionDate,
                Amount = accountDetail.Amount,
                OperationTypeId = accountDetail.OperationTypeId
            };
        }

        private abstract class ImportFlowFinancialData1CHandlerSpecsContext
        {
            private Establish context = () =>
                {
                    Mocks = new MockRepository(MockBehavior.Strict);

                    ClientProxyFactory = Mocks.Create<IClientProxyFactory>();
                    IntegrationSettings = Mocks.Create<IIntegrationSettings>();
                    Logger = new Mock<ICommonLog>();
                    AppSettings = Mocks.Create<IAppSettings>();
                    EmployeeEmailResolver = Mocks.Create<IEmployeeEmailResolver>();
                    NotificationSender = Mocks.Create<INotificationSender>();
                    ScopeFactory = Mocks.Create<IOperationScopeFactory>();
                    AccountRepository = new FakeAccountRepository();

                    Handler = new ImportFlowFinancialData1CHandler(
                        ClientProxyFactory.Object,
                        IntegrationSettings.Object,
                        Logger.Object,
                        AccountRepository,
                        AppSettings.Object,
                        EmployeeEmailResolver.Object,
                        NotificationSender.Object,
                        ScopeFactory.Object);

                    BrokerApiReceiver = new FakeBrokerApiReceiver();
                    ClientProxy = new FakeClientProxy(BrokerApiReceiver);
                    OperationScope = Mocks.Create<IOperationScope>();
            
                    ClientProxyFactory.Setup(x => x.GetClientProxy<IBrokerApiReceiver>(Moq.It.IsAny<string>())).Returns(ClientProxy);
                    IntegrationSettings.Setup(x => x.IntegrationApplicationName).Returns(string.Empty);
                };

            protected static MockRepository Mocks { get; private set; }
            protected static Mock<IClientProxyFactory> ClientProxyFactory { get; private set; }
            protected static Mock<IIntegrationSettings> IntegrationSettings { get; private set; }
            protected static Mock<ICommonLog> Logger { get; private set; }
            protected static Mock<IAppSettings> AppSettings { get; private set; }
            protected static Mock<IEmployeeEmailResolver> EmployeeEmailResolver { get; private set; }
            protected static Mock<INotificationSender> NotificationSender { get; private set; }
            protected static Mock<IOperationScopeFactory> ScopeFactory { get; private set; }
            protected static FakeAccountRepository AccountRepository { get; private set; }

            protected static ImportFlowFinancialData1CHandler Handler { get; private set; }

            protected static FakeBrokerApiReceiver BrokerApiReceiver { get; private set; }
            protected static FakeClientProxy ClientProxy { get; private set; }
            protected static Mock<IOperationScope> OperationScope { get; private set; }
        }

        private abstract class ImportFlowFinancialData1CHandlerSpecsReceivePackageContext : ImportFlowFinancialData1CHandlerSpecsContext
        {
            private Establish context = () =>
                {
                    OperationScope.Setup(x => x.Dispose());
                    OperationScope.Setup(x => x.Complete()).Returns(OperationScope.Object);
                    ScopeFactory.Setup(x => x.CreateNonCoupled<ImportFlowFinancialData1CIdentity>()).Returns(OperationScope.Object);
                };
        }

        private abstract class ImportFlowFinancialData1CHandlerSpecsReceiveOperationsContext : ImportFlowFinancialData1CHandlerSpecsReceivePackageContext
        {
            private Establish context = () => 
                AppSettings.Setup(x => x.EnableNotifications).Returns(false);
        }

        private sealed class When_receive_empty_package : ImportFlowFinancialData1CHandlerSpecsContext
        {
            private static Response response;

            private Because of = () =>
                {
                    response = Handler.Handle(new ImportFlowFinancialData1CRequest());
                };

            private It should_returns_empty_response = () => response.Should().Be(Response.Empty);
            private It should_call_brokerAPI_BeginReceiving_method = () => BrokerApiReceiver.IsBeginReceivingCalled.Should().BeTrue();
            private It should_call_brokerAPI_EndReceiving_method = () => BrokerApiReceiver.IsEndReceivingCalled.Should().BeTrue();
            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();
        }

        private sealed class When_receive_package_without_operations : ImportFlowFinancialData1CHandlerSpecsReceivePackageContext
        {
            private static Response response;

            private Establish context = () =>
                {
                    var packages = new[] { DoubleGis.Erm.BLCore.Tests.Unit.Properties.Resources.ImportFlowFinancialData1CHandlerTestFixture_NoOperationPackage };
                    BrokerApiReceiver.ReceivedPackages = packages;
                };

            private Because of = () =>
                {
                    response = Handler.Handle(new ImportFlowFinancialData1CRequest());
                };

            private It should_returns_empty_response = () => response.Should().Be(Response.Empty);
            private It should_call_brokerAPI_BeginReceiving_method = () => BrokerApiReceiver.IsBeginReceivingCalled.Should().BeTrue();
            private It should_call_brokerAPI_EndReceiving_method = () => BrokerApiReceiver.IsEndReceivingCalled.Should().BeTrue();
            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();
        }

        private sealed class When_receive_package_with_single_operation : ImportFlowFinancialData1CHandlerSpecsReceiveOperationsContext
        {
            // как в принимаемом сообщении
            const long ACCOUNT_ID = 224075643356349704;
            const bool IS_PLUS = true; 
            const string SYNC_CODE_1C = "004"; 
            const int OPERATION_TYPE_CODE_1C = 1;
            const int AMOUNT = 10000;

            private static Response response;
            private static object[] expectedDeletedAccountDetails;
            private static object[] expectedCreatedAccountDetails;
            private static long[] expectedUpdatedBalanceAccountIds;

            private Establish context = () =>
                {
            var transactionDate = DateTime.Parse("2013-11-06T10:58:52");

            var accountDetail = new AccountDetail();

            var accountInfo = new AccountRepository.AccountInfoForImportFrom1C
                {
                    Id = ACCOUNT_ID,
                    BranchOfficeSyncCode1C = SYNC_CODE_1C,
                    AccountDetails = new[] { accountDetail }
                };

            var operations = new[]
                {
                    new OperationType
                        {
                            Id = OPERATION_TYPE_CODE_1C,
                            IsPlus = IS_PLUS, 
                            SyncCode1C = OPERATION_TYPE_CODE_1C.ToString(CultureInfo.InvariantCulture)
                        }
                };

            expectedUpdatedBalanceAccountIds = new[] { ACCOUNT_ID };
                    expectedDeletedAccountDetails = new object[] { Simplify(accountDetail) };
                    expectedCreatedAccountDetails = new object[]
                { 
                    new SimplifiedAccountDetail
                    {
                        AccountId = ACCOUNT_ID,
                        OperationTypeId = OPERATION_TYPE_CODE_1C,
                        Amount = AMOUNT,
                        TransactionDate = transactionDate
                    }
                };

                    AccountRepository.OperationsInSyncWith1C.AddRange(operations);
                    AccountRepository.AccountInfos.Add(accountInfo);

                    BrokerApiReceiver.ReceivedPackages = new[] { DoubleGis.Erm.BLCore.Tests.Unit.Properties.Resources.ImportFlowFinancialData1CHandlerTestFixture_SingleOperationPackage };
                };
            
            private Because of = () => 
                response = Handler.Handle(new ImportFlowFinancialData1CRequest());

            private It should_returns_empty_response = () => response.Should().Be(Response.Empty);
            private It should_call_brokerAPI_BeginReceiving_method = () => BrokerApiReceiver.IsBeginReceivingCalled.Should().BeTrue();
            private It should_call_brokerAPI_Acknowledge_method = () => BrokerApiReceiver.IsAcknowledgeCalled.Should().BeTrue();
            private It should_call_brokerAPI_EndReceiving_method = () => BrokerApiReceiver.IsEndReceivingCalled.Should().BeTrue();
            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();

            private It should_delete_right_account_details =
                () => AccountRepository.DeletedAccountDetails.Select(Simplify).Should().BeEquivalentTo(expectedDeletedAccountDetails);
            
            private It should_create_right_account_details =
                () => AccountRepository.CreatedAccountDetails.Select(Simplify).Should().BeEquivalentTo(expectedCreatedAccountDetails);

            private It should_update_right_account_ids =
                () => AccountRepository.UpdatedAccountIds.Should().BeEquivalentTo(expectedUpdatedBalanceAccountIds);
        }

        private sealed class When_receive_package_with_multiple_operations : ImportFlowFinancialData1CHandlerSpecsReceiveOperationsContext
        {
            // как в принимаемом сообщении
            const long ACCOUNT_ID_1 = 224075643356349704;
            const long ACCOUNT_ID_2 = 224075811002689544;
            const bool OPERATION_1_IS_PLUS = true;
            const bool OPERATION_2_IS_PLUS = false;
            const string SYNC_CODE_1C = "004";
            const int OPERATION_TYPE_CODE_1C = 1;
            const int OPERATION_TYPE_CODE_1C_2 = 2;
            const int AMOUNT_1 = 10000;
            const int AMOUNT_2 = 100;
            const int AMOUNT_3 = 1000;

            private static Response response;
            private static object[] expectedDeletedAccountDetails;
            private static object[] expectedCreatedAccountDetails;
            private static long[] expectedUpdatedBalanceAccountIds;

            private Establish context = () =>
                {
            var transactionDate_1 = DateTime.Parse("2013-11-06T10:58:52");
            var transactionDate_2 = DateTime.Parse("2013-11-06T10:58:59");
            
            var accountDetail_1 = new AccountDetail();
            var accountDetail_2 = new AccountDetail();
            var accountDetail_3 = new AccountDetail();

            var accountInfo_1 = new AccountRepository.AccountInfoForImportFrom1C
            {
                Id = ACCOUNT_ID_1,
                BranchOfficeSyncCode1C = SYNC_CODE_1C,
                AccountDetails = new[] { accountDetail_1, accountDetail_2 }
            };

            var accountInfo_2 = new AccountRepository.AccountInfoForImportFrom1C
            {
                Id = ACCOUNT_ID_2,
                BranchOfficeSyncCode1C = SYNC_CODE_1C,
                AccountDetails = new[] { accountDetail_3 }
            };

            expectedUpdatedBalanceAccountIds = new[] { ACCOUNT_ID_1, ACCOUNT_ID_2 };

                    expectedDeletedAccountDetails = new object[]
                {
                    Simplify(accountDetail_1), 
                    Simplify(accountDetail_2), 
                    Simplify(accountDetail_2)
                };

                    expectedCreatedAccountDetails = new object[]
                { 
                    new SimplifiedAccountDetail
                    {
                        AccountId = ACCOUNT_ID_1,
                        OperationTypeId = OPERATION_TYPE_CODE_1C,
                        Amount = AMOUNT_1,
                        TransactionDate = transactionDate_1
                    },
                    new SimplifiedAccountDetail
                    {
                        AccountId = ACCOUNT_ID_1,
                        OperationTypeId = OPERATION_TYPE_CODE_1C_2,
                        Amount = AMOUNT_2,
                        TransactionDate = transactionDate_2
                    },
                    new SimplifiedAccountDetail
                    {
                        AccountId = ACCOUNT_ID_2,
                        OperationTypeId = OPERATION_TYPE_CODE_1C,
                        Amount = AMOUNT_3,
                        TransactionDate = transactionDate_1
                    }
                };

                    AccountRepository.OperationsInSyncWith1C.AddRange(new[]
                {
                    GetOperationType(OPERATION_TYPE_CODE_1C, OPERATION_1_IS_PLUS, OPERATION_TYPE_CODE_1C),
                    GetOperationType(OPERATION_TYPE_CODE_1C_2, OPERATION_2_IS_PLUS, OPERATION_TYPE_CODE_1C_2)
                });
                    AccountRepository.AccountInfos.Add(accountInfo_1);
                    AccountRepository.AccountInfos.Add(accountInfo_2);

                    var packages = new[] { DoubleGis.Erm.BLCore.Tests.Unit.Properties.Resources.ImportFlowFinancialData1CHandlerTestFixture_MultipleOperationPackage };
                    BrokerApiReceiver.ReceivedPackages = packages;
                };
            
            private Because of = () =>
                response = Handler.Handle(new ImportFlowFinancialData1CRequest());

            private It should_returns_empty_response = () => response.Should().Be(Response.Empty);
            private It should_call_brokerAPI_BeginReceiving_method = () => BrokerApiReceiver.IsBeginReceivingCalled.Should().BeTrue();
            private It should_call_brokerAPI_Acknowledge_method = () => BrokerApiReceiver.IsAcknowledgeCalled.Should().BeTrue();
            private It should_call_brokerAPI_EndReceiving_method = () => BrokerApiReceiver.IsEndReceivingCalled.Should().BeTrue();
            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();

            private It should_delete_right_account_details =
                () => AccountRepository.DeletedAccountDetails.Select(Simplify).Should().BeEquivalentTo(expectedDeletedAccountDetails);
            
            private It should_create_right_account_details =
                () => AccountRepository.CreatedAccountDetails.Select(Simplify).Should().BeEquivalentTo(expectedCreatedAccountDetails);

            private It should_update_right_account_ids =
                () => AccountRepository.UpdatedAccountIds.Should().BeEquivalentTo(expectedUpdatedBalanceAccountIds);
        }

        private class When_receive_package_with_unknown_account_code : ImportFlowFinancialData1CHandlerSpecsContext
        {
            private static Exception exception;

            private Establish context = () =>
        {
                    OperationScope.Setup(x => x.Dispose());
                    ScopeFactory.Setup(x => x.CreateNonCoupled<ImportFlowFinancialData1CIdentity>()).Returns(OperationScope.Object);

            // как в принимаемом сообщении
            const bool IS_PLUS = true;
            const int OPERATION_TYPE_CODE_1C = 1;

            var operations = new[]
                {
                    new OperationType
                        {
                            Id = OPERATION_TYPE_CODE_1C,
                            IsPlus = IS_PLUS, 
                            SyncCode1C = OPERATION_TYPE_CODE_1C.ToString(CultureInfo.InvariantCulture)
                        }
                };

                    AccountRepository.OperationsInSyncWith1C.AddRange(operations);

                    BrokerApiReceiver.ReceivedPackages = new[]
                {
                            DoubleGis.Erm.BLCore.Tests.Unit.Properties.Resources.ImportFlowFinancialData1CHandlerTestFixture_UnknownAccountOperationPackage
                        };
                };

            private Because of = () => Catch.Exception(() => Handler.Handle(new ImportFlowFinancialData1CRequest()));

            private It should_not_call_brokerAPi_Acknowledge_method = () => BrokerApiReceiver.IsAcknowledgeCalled.Should().BeFalse();
            private It should_call_brokerAPI_BeginReceiving_method = () => BrokerApiReceiver.IsBeginReceivingCalled.Should().BeTrue();
            private It should_call_brokerAPI_EndReceiving_method = () => BrokerApiReceiver.IsEndReceivingCalled.Should().BeTrue();
            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();
        }

        private sealed class SimplifiedAccountDetail
        {
            public long Id { get; set; }
            public long AccountId { get; set; }
            public DateTime TransactionDate { get; set; }
            public decimal Amount { get; set; }
            public long OperationTypeId { get; set; }
            
            public override string ToString()
            {
                return string.Format("{5}Id = {0}, AccountId = {1}, OperationTypeId = {2}, Amount = {3}, TransactionDate = {4}",
                    Id,
                    AccountId,
                    OperationTypeId,
                    Amount,
                    TransactionDate,
                    Environment.NewLine);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                return obj.GetType() == GetType() && Equals((SimplifiedAccountDetail)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Id.GetHashCode();
                    hashCode = (hashCode * 397) ^ AccountId.GetHashCode();
                    hashCode = (hashCode * 397) ^ TransactionDate.GetHashCode();
                    hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                    hashCode = (hashCode * 397) ^ OperationTypeId.GetHashCode();
                    return hashCode;
                }
            }

            private bool Equals(SimplifiedAccountDetail other)
            {
                return Id == other.Id
                    && AccountId == other.AccountId
                    && TransactionDate.Equals(other.TransactionDate)
                    && Amount == other.Amount
                    && OperationTypeId == other.OperationTypeId;
            }
        }

        private class FakeClientProxy : IClientProxy<IBrokerApiReceiver>
        {
            private readonly FakeBrokerApiReceiver _brokerApiReceiver;

            public FakeClientProxy(FakeBrokerApiReceiver brokerApiReceiver)
            {
                _brokerApiReceiver = brokerApiReceiver;
            }

            public void Execute(Action<IBrokerApiReceiver> action)
            {
                action(_brokerApiReceiver);
            }

            public TResult Execute<TResult>(Func<IBrokerApiReceiver, TResult> func)
            {
                throw new NotImplementedException();
            }

            public bool TryExecuteWithFaultContract<TResult>(Func<IBrokerApiReceiver, TResult> func, out TResult result, out object faultContract)
            {
                throw new NotImplementedException();
            }
        }

        private class FakeBrokerApiReceiver : IBrokerApiReceiver
        {
            public string[] ReceivedPackages { get; set; }
            public bool IsBeginReceivingCalled { get; private set; }
            public bool IsAcknowledgeCalled { get; private set; }
            public bool IsEndReceivingCalled { get; private set; }

            public void BeginReceiving(string appCode, string messageType)
            {
                IsBeginReceivingCalled = true;
            }

            public string[] ReceivePackage()
            {
                return IsAcknowledgeCalled ? null : ReceivedPackages;
            }

            public void Acknowledge()
            {
                IsAcknowledgeCalled = true;
            }

            public void EndReceiving()
            {
                IsEndReceivingCalled = true;
            }
        }
    }
}
