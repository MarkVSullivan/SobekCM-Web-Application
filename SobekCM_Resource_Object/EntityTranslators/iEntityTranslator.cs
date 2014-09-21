using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.EntityTranslators
{
    public interface IEntityTranslator
    {
        bool CanTranslate(Type targetType, Type sourceType);
        bool CanTranslate<TTarget, TSource>();
        object Translate(IEntityTranslatorService service, Type targetType, object source);
        TTarget Translate<TTarget>(IEntityTranslatorService service, object source);
    }
}
