using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.UsCore
{
  /// <summary>
  /// Class with utility functions for Us Core Patient operations.
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-patient.html
  /// </summary>
  public static class UsCorePatient
  {
    /// <summary>
    /// Official Profile URL for US Core Patient.
    /// </summary>
    public const string ProfileUrl = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient";

    /// <summary>
    /// Set the assertion that a patient object is conforming to the US Core Patient Profile.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="addConformance">True to assert conformance, false to remove the assertion.</param>
    public static void UsCorePatientSetProfileConformance(
      this Patient patient,
      bool addConformance = true)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      // check to see if there is no Meta
      if (patient.Meta == null)
      {
        if (!addConformance)
        {
          return;
        }

        patient.Meta = new Meta();
      }

      if ((patient.Meta.Profile == null) || (patient.Meta.Profile.Count() == 0))
      {
        if (!addConformance)
        {
          return;
        }

        patient.Meta.Profile = new List<string>()
        {
          ProfileUrl,
        };

        return;
      }

      // check if the profile exists
      if (patient.Meta.Profile.Contains(ProfileUrl))
      {
        if (addConformance)
        {
          // don't need to add
          return;
        }

        if (!addConformance)
        {
          // remove
          ((List<string>)patient.Meta.Profile).Remove(ProfileUrl);
          return;
        }
      }

      if (!addConformance)
      {
        // not present and don't add it
        return;
      }

      // add this profile
      ((List<string>)patient.Meta.Profile).Add(ProfileUrl);
    }
  }
}