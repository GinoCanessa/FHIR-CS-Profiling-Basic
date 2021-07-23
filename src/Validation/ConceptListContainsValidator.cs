using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.Validation
{
  /// <summary>
  /// Validator to see if a list of codeable concepts contains a value with a specific system and code
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="TCollectionElement"></typeparam>
  public class ConceptListContainsValidator<T, TCollectionElement>
    : PropertyValidator<T, IList<TCollectionElement>>
    where TCollectionElement : CodeableConcept
  {
    /// <summary>
    /// System we are looking for
    /// </summary>
    private string _system;

    /// <summary>
    /// Code we are looking for
    /// </summary>
    private string _code;

    /// <summary>
    /// Name of this validator for error messages
    /// </summary>
    public override string Name => "ConceptListContainsValidator";

    /// <summary>
    /// Create a ConceptListContainsValidator
    /// </summary>
    /// <param name="system"></param>
    /// <param name="code"></param>
    public ConceptListContainsValidator(string system, string code)
    {
      _system = system;
      _code = code;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="conceptList"></param>
    /// <returns></returns>
    public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> conceptList)
    {
      if ((conceptList == null) || (!conceptList.Any()))
      {
        context.MessageFormatter.AppendArgument("DoesNotContain", $"{_system}#{_code}");
        return false;
      }

      foreach (CodeableConcept concept in conceptList)
      {
        if ((concept.Coding == null) || (!concept.Coding.Any()))
        {
          continue;
        }

        if (concept.Coding.Any(coding => (coding.System == _system) && (coding.Code == _code)))
        {
          return true;
        }
      }

      context.MessageFormatter.AppendArgument("DoesNotContain", $"{_system}#{_code}");
      return false;
    }

    /// <summary>
    /// Get the message template for errors
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    protected override string GetDefaultMessageTemplate(string errorCode) =>
      $"{{PropertyName}} must contain a CodeableConcept with a code matching: {_system}#{_code}.";
  }
}