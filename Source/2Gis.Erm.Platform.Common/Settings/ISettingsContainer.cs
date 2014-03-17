﻿using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    /// <summary>
    /// Маркерный интерфейс для типа содержащего определенный набор настроек, часть из которых может быть вынесена в аспекты
    /// Данный интерфейс, реализуют только конкретные классы - контейнеры настроек (другие интерфейсы не должны расширять его).
    /// Основные соглашения/требования для классов контейнеров настроек:
    /// - контейнер настроек четко связан с точкой входа в приложение (т.е. не переиспользуется и т.п.)
    /// - контейнер это просто контейнер, его статичейский контракт (контракт класса) не имеет никакого прикладного применения - цель его существования всего лишь объединять настройки
    /// - главное требование к его коду - это высокая читаемость, т.к. контейнер настроек фактически фиксирует требования функционала, к набору настроек который должен быть в конфиге
    /// - никаких мега агрегирующих интерфейсов он реализовавать не должен
    /// - конкретные контракты настроек (расширяющие ISettings), реализуются непосредственно контейнером настроек (через explicit interface implementaion), 
    ///     только если данные контракты настроек больше не используются другими контейнерами (т.е. настройки используются только функционалом доступным через единственную точку входа)
    /// - все конкретные контракты настроек, необходимые в нескольких точках входа, выносятся в аспекты настроек, контейнер настроек только подключает себе все нужные аспекты, т.о.
    ///     если настройки начинают использоваться несколькими точками входа - необходимо вынести их в аспект, исключив явную реализацию контейнером настроек
    /// ЗАМЕЧАНИЕ: хотя данный гибридный подход (часть настроек реализуется явно, а часть подключается через аспекты), кажется несколько неоднозначным, главное его преимущество это простота и скорость поддержки изменений
    /// конкурирующие варианты были признаны нецелесообразными для конкретной ситуации:
    ///     1). все вынести в аспекты - необходимость плодить отдельные классы реализации аспектов даже для настроек, переиспользование которых в других точках входа не нужно, 
    ///         при этом сам факт существования аспекта подразумевает возможность переиспользования (чего довольно легко достичь, случайно зареференсив сборку + добавив using)
    ///         Чтобы более явно разделить аспекты на локальные (эксклюзивные) и переиспользуемые пришлось бы вносить знания о разной природе аспектов в инфраструктуру конфигов, 
    ///         многие проверки были бы возможны только в runtime (по типу проверки ресурсникоа, либо в massprocessor).
    ///         Итого, явное усложнение, при сомнительных преимуществах, в данной конкретной ситуации
    ///     2). реализовать все контракты настроек непосредственно в контейнере настроек - нарушение DRY, дофига copy/paste, гемор с поддержкой, читабельностью и т.п.
    /// </summary>
    public interface ISettingsContainer : ISettings
    {
        IEnumerable<ISettingsAspect> SettingsAspects { get; }
    }
}