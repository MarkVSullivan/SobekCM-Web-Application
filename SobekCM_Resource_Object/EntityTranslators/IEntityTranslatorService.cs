using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.EntityTranslators
{
    /// <summary>
    ///
    /// </summary>
    public interface IEntityTranslatorService
    {

        bool CanTranslate(Type targetType, Type sourceType);

        /// <summary>
        /// Determines if a given type can be translated to a target type.
        /// </summary>
        /// <typeparam name="TTarget">
        /// Target type.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// Source type.
        /// </typeparam>
        /// <returns>
        /// True if a translator exists for the provided types.
        /// </returns>
        bool CanTranslate<TTarget, TSource>();

        /// <summary>
        /// Translate a source object to a target type.
        /// </summary>
        /// <param name="targetType">Target type to translate to.</param>
        /// <param name="source">Source object to be translated.</param>
        /// <returns>
        /// New object translated from <c>source</c> to <c>targetType</c>.
        /// </returns>
        object Translate(Type targetType, object source);

        /// <summary>
        /// Translate an object to a desired type.
        /// </summary>
        /// <typeparam name="TTarget">
        /// Type to translate object to.
        /// </typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        TTarget Translate<TTarget>(object source);

        /// <summary>
        /// Register a translator with the translator service.
        /// </summary>
        /// <param name="translator">Translator to register.</param>
        void RegisterEntityTranslator(IEntityTranslator translator);

        /// <summary>
        /// Remove a translator from the service.
        /// </summary>
        /// <param name="translator">Translator to remove.</param>
        void RemoveEntityTranslator(IEntityTranslator translator);

    }
}
