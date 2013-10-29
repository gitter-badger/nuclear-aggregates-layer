# language:en

@DAL
Feature: UnitOfWork
	
	@UnityUnitOfWork
	Scenario: UnityUnitOfWork instantiating
		Given create instance of UnityUnitOfWork class
		Then UnityUnitOfWork instance should not be null
		Then ScopeId should not be Guid.Empty

	@StubUnitOfWork
	Scenario: Testing CreateRepository method with concrete aggregate repository type as a parameter
		Given create instance of StubUnitOfWork class
		When call CreateRepository with concrete aggregate repository type as a parameter using test action
		Then exception shoud not be thrown

	@StubUnitOfWork
	Scenario: Testing CreateRepository method with incorrect type as a parameter
		Given create instance of StubUnitOfWork class
		When call CreateRepository with incorrect type as a parameter using test action
		Then ArgumentException shoud be thrown

	@MockUnitOfWork
	Scenario: Testing CreateRepository for proxies
		Given create instance of MockUnitOfWork class
		When call CreateRepository with aggregate repository interface type as a parameter
		Then passed parameters are instances of expected classes
	