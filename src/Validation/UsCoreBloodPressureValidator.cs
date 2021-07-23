using System;
using System.Collections.Generic;
using System.Linq;
using fhir_cs_profiling_basic.UsCore;
using FluentValidation;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.Validation
{
  /// <summary>
  /// Class to validate Observation Objects against the US Core Blood Pressure Profile
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-blood-pressure.html
  /// </summary>
  public class UsCoreBloodPressureValidator : AbstractValidator<Observation>
  {
    /// <summary>
    /// Create a default instance of the US Core Blood Pressure Validator
    /// </summary>
    public UsCoreBloodPressureValidator()
    {
      // validate against US Core Vital Signs
      RuleFor(observation => observation)
        .SetValidator(new UsCoreVitalSignsValidator());
      
      RuleFor(observation => observation.Code)
        .ConceptContains(UsCoreBloodPressure.UrlCodeSystemLoinc, UsCoreBloodPressure.LoincCodeBloodPressurePanel);

      RuleFor(observation => observation.Component)
        .Must(list =>
          list.Any(component =>
            component.Code.Coding.Any(coding =>
              (coding.System == UsCoreBloodPressure.UrlCodeSystemLoinc) &&
              (coding.Code == UsCoreBloodPressure.LoincCodeSystolic))))
        .WithMessage($"UsCoreBloodPressure requires a component: {UsCoreBloodPressure.UrlCodeSystemLoinc}#{UsCoreBloodPressure.LoincCodeSystolic}")
        .Must(list =>
          list.Any(component =>
            component.Code.Coding.Any(coding =>
              (coding.System == UsCoreBloodPressure.UrlCodeSystemLoinc) &&
              (coding.Code == UsCoreBloodPressure.LoincCodeDiastolic))))
        .WithMessage($"UsCoreBloodPressure requires a component: {UsCoreBloodPressure.UrlCodeSystemLoinc}#{UsCoreBloodPressure.LoincCodeDiastolic}");
    }
  }
}