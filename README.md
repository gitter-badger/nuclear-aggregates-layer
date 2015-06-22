![NuClear](media/nuclear-logo.png)
# NuClear Aggeregates Layer

[![Join the chat at https://gitter.im/2gis/nuclear-aggregates-layer](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/2gis/nuclear-aggregates-layer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
_NuClear is the set of 2GIS projects used internally and open for contribution_

This repo contains source code of 3 logical packages - **Storage**, **Aggregates** and **DI-package** for composing them in runtime. 

##Storage
_Basic abtractions for interactions with storages. Consists of separated Read and Write stacks._

There are two concrete imlementations - with [Entity Framework 6][ef6], and with [LinqToDB][linq-to-db]

TODO: present all storage abstractions and basic implementations

##Aggregates
_Basic abstractions for aggregate services implementations._

As well as Storage it contains readonly abstrations and abstrations for writings that must be invariant-safe.

TODO: present all storage abstractions

##DI-package
_There is runtime factories and processors used for search and register storage and aggregate services implementations with [Unity][unity]_

TODO: present how these factories works

[ef6]: http://entityframework.codeplex.com/
[linq-to-db]: https://github.com/linq2db/linq2db
[unity]: [https://unity.codeplex.com/]

