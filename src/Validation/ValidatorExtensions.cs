using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.Validation
{
  /// <summary>
  /// Class to hold validator extensions
  /// </summary>
  public static class ValdiatorExtension
  {
    /// <summary>
    /// Validator to see if a list of codeable concepts contains a value with a specific system and code
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <param name="system"></param>
    /// <param name="code"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, IList<TElement>> ConceptListContains<T, TElement>(
      this IRuleBuilder<T, IList<TElement>> ruleBuilder,
      string system,
      string code)
      where TElement : CodeableConcept
    {
      return ruleBuilder.SetValidator(new ConceptListContainsValidator<T, TElement>(system, code));
    }

    /// <summary>
    /// Validator to see if a codeable concepts contains a value with a specific system and code
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <param name="system"></param>
    /// <param name="code"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TElement> ConceptContains<T, TElement>(
      this IRuleBuilder<T, TElement> ruleBuilder,
      string system,
      string code)
      where TElement : CodeableConcept
    {
      return ruleBuilder.SetValidator(new ConceptContainsValidator<T, TElement>(system, code));
    }
  }
}