using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers
{
    // FIXME {all, 07.04.2014}: не совсем SRP контракт + не понятно место данной абстракции в модели ERM, если это расширение обычно obtainer, то
    // обычный obtainer - это фактически одна из операций readmodel, реализованная не в основоном типе readmodel, а отдельном
    // фактически у него ответсвенность, достать из хранилища и сделать mapping свойств, то что пока mapping свойств сделан явно в самом же obtainer, не говорит о том, что это хорошо, дальше возможно будет использована композиция + какой-нибудь automapper, valueinjecter и т.п.
    // влюбом случай после уточнения ответственности либо данный тип станет не нужен (будет разделен), либо у него какое-то более четкое имя будет
    public interface IBusinessModelEntityObtainerFlex<in TEntity> where TEntity : IEntity, IEntityKey, IPartable
    {
        void CopyPartFields(TEntity target, IDomainEntityDto dto);

        IEntityPart[] GetEntityParts(TEntity entity);
    }
}