using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Docs;

using FluentAssertions;

using Machine.Specifications;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticAdvancedQueryingIntegrationSpecs
    {
        [Subject(typeof(ElasticClient))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_action : ElasticApiIntegrationContext
        {
            Establish context = () =>
                {
                    ElasticManagementApi.CreateIndex<ClientGridDoc>(ElasticTestConfigurator.GetTestIndexDescriptor());

                    var id = 0;
                    IndexClientGridDocWithOwnerCode(id++, 1);
                    IndexClientGridDocWithOwnerCode(id++, 11);
                    IndexClientGridDocWithOwnerCode(id++, 11);
                    IndexClientGridDocWithOwnerCode(id++, 2);
                    IndexClientGridDocWithOwnerCode(id++, 2);
                    IndexClientGridDocWithOwnerCode(id++, 22);
                    IndexClientGridDocWithOwnerCode(id++, 22);
                    IndexClientGridDocWithOwnerCode(id++, 3);
                    IndexClientGridDocWithOwnerCode(id, 33);

                    ElasticApi.Refresh();

                    InUnion = new[] { "2", "1", "33" };
                };

            Because of = () => Result = ElasticApi.Search<ClientGridDoc>(sd => sd.Query(q => q.Terms(c => c.OwnerCode, InUnion)).From(0).Size(1000));

            It should_valid = () => Result.IsValid.Should().BeTrue();
            It should_have_same_count_as_searched = () => Result.Documents.Count().Should().Be(4);

            Cleanup cleanup = () => ElasticManagementApi.DeleteIndex<ClientGridDoc>();

            static void IndexClientGridDocWithOwnerCode(long id, long ownerCode)
            {
                ElasticApi.Index(new ClientGridDoc { Id = id.ToString(), OwnerCode = ownerCode.ToString() });
            }

            static ISearchResponse<ClientGridDoc> Result;
            static string[] InUnion;
        }

        [Subject(typeof(ElasticClient))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_query_clients_with_dept : ElasticApiIntegrationContext
        {
            Establish context = () =>
                {
                    ElasticManagementApi.CreateIndex<ClientGridDoc>(ElasticTestConfigurator.GetTestIndexDescriptor());

                    MapClientWithLegalPersonsWithAccounts();

                    ExpectedClientId = "42";

                    const bool active = true;
                    const bool notDeleted = false;

                    BalanceLimit = 1.0;
                    const double deptBalance = 0.99;

                    IndexClientWithLegalPersonWithAccount(ExpectedClientId, active, notDeleted, "1", active, notDeleted, "1", active, notDeleted, deptBalance);

                    IndexClientWithLegalPersonWithAccount("1", active, notDeleted, "1", active, notDeleted, "1", !active, notDeleted, deptBalance);
                    IndexClientWithLegalPersonWithAccount("2", active, notDeleted, "1", active, notDeleted, "1", active, !notDeleted, deptBalance);

                    IndexClientWithLegalPersonWithAccount("3", active, notDeleted, "1", !active, notDeleted, "1", active, notDeleted, deptBalance);
                    IndexClientWithLegalPersonWithAccount("4", active, notDeleted, "1", active, !notDeleted, "1", active, notDeleted, deptBalance);

                    IndexClientWithLegalPersonWithAccount("5", !active, notDeleted, "1", active, notDeleted, "1", active, notDeleted, deptBalance);
                    IndexClientWithLegalPersonWithAccount("6", active, !notDeleted, "1", active, notDeleted, "1", active, notDeleted, deptBalance);

                    IndexClientWithLegalPersonWithAccount("7", active, notDeleted, "1", active, notDeleted, "1", active, notDeleted, BalanceLimit);
                };

            Because of = () => Result = ElasticApi.Search<ClientGridDoc>(sd => sd.Filter(f =>
                    f.And(
                        acc => acc.Nested(n => n.Path(c => c.LegalPersons.First().Accounts)
                                .Query(nq => nq.Bool(b => b.Must(
                                        accBalance => accBalance.Range(r => r.OnField(c => c.LegalPersons[0].Accounts[0].Balance).Lower(BalanceLimit)),
                                        accIsActive => accIsActive.Term(c => c.LegalPersons[0].Accounts[0].IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.LegalPersons[0].Accounts[0].IsDeleted, false)
                                    )))),
                        lp => lp.Nested(n => n.Path(c => c.LegalPersons)
                                .Query(nq => nq.Bool(b => b.Must(
                                        accIsActive => accIsActive.Term(c => c.LegalPersons[0].IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.LegalPersons[0].IsDeleted, false)
                                    )))),
                        cl => cl.Query(nq => nq.Bool(b => b.Must(
                                        accIsActive => accIsActive.Term(c => c.IsActive, true),
                                        accIsDeleted => accIsDeleted.Term(c => c.IsDeleted, false)
                                    )))
                    )
                ));

            It should_valid = () => Result.IsValid.Should().BeTrue();
            It should_find_client_with_dept = () => Result.Documents.ToArray().Should().OnlyContain(c => c.Id == ExpectedClientId);

            Cleanup cleanup = () => ElasticManagementApi.DeleteIndex<ClientGridDoc>();

            static string ExpectedClientId;
            static ISearchResponse<ClientGridDoc> Result;
            static double BalanceLimit;

            static void MapClientWithLegalPersonsWithAccounts()
            {
                ElasticManagementApi.Map<ClientGridDoc>(m => m
                    .Properties(p => p
                        .Boolean(b => b.Name(n => n.IsActive))
                        .Boolean(b => b.Name(n => n.IsDeleted))
                        .NestedObject<LegalPersonDoc>(no => no
                            .Name(n => n.LegalPersons.First())
                            .Properties(pp => pp
                                .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                .Boolean(b => b.Name(n => n.IsActive))
                                .Boolean(b => b.Name(n => n.IsDeleted))
                                .NestedObject<AccountDoc>(noo => noo
                                    .Name(n => n.Accounts.First())
                                    .Properties(ppp => ppp
                                        .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                        .Number(s => s.Name(n => n.Balance).Type(NumberType.@double))
                                        .Boolean(b => b.Name(n => n.IsActive))
                                        .Boolean(b => b.Name(n => n.IsDeleted))
                                    )
                                )
                            )
                        )
                    )
                );
            }

            static void IndexClientWithLegalPersonWithAccount(string clientId, bool clientActive, bool clientDeleted,
                                                                string legalPersonId, bool persActive, bool persDeleted,
                                                                string accountId, bool accActive, bool accDeleted, double balance)
            {
                var client = new ClientGridDoc
                {
                    Id = clientId,
                    IsActive = clientActive,
                    IsDeleted = clientDeleted,

                    LegalPersons = new[]
                            {
                                new LegalPersonDoc
                                    {
                                        Id = legalPersonId,
                                        IsActive = persActive,
                                        IsDeleted = persDeleted,

                                        Accounts = new []
                                            {
                                                new AccountDoc
                                                    {
                                                        Id = accountId,
                                                        Balance = balance,
                                                        IsActive = accActive,
                                                        IsDeleted = accDeleted,
                                                    }, 
                                            }
                                    }
                            }
                };

                ElasticApi.Index(client);
                ElasticApi.Refresh();
            }
        }
    }
}