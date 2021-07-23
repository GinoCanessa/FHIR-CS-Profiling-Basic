using System;
using System.Collections.Generic;
using System.Linq;
using fhir_cs_profiling_basic.UsCore;
using FluentValidation;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.Validation
{
  /// <summary>
  /// Class to validate Observation Objects against the US Core Vital Signs Profile
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-vital-signs.html
  /// </summary>
  public class UsCoreVitalSignsValidator : AbstractValidator<Observation>
  {
    /// <summary>
    /// Create a default instance of the US Core Vital Signs Validator
    /// </summary>
    public UsCoreVitalSignsValidator()
    {
      RuleFor(observation => observation.Category)
        .ConceptListContains(UsCoreVitalSigns.UrlCodeSystemObservationCategory, UsCoreVitalSigns.ObservationCategoryVitalSigns);
    }
  }
}