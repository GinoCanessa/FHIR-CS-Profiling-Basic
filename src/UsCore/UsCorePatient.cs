using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.UsCore
{
  /// <summary>
  /// Class with Patient extensions for US Core Patient objects
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-patient.html
  /// </summary>
  public static class UsCorePatient
  {
    /// <summary>
    /// The official URL for the US Core Patient profile, used to assert conformance.
    /// </summary>
    public const string ProfileUrl = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient";

    /// <summary>
    /// Set the assertion that a patient object conforms to the US Core Patient Profile.
    /// </summary>
    /// <param name="patient"></param>
    public static void UsCorePatientProfileSet(this Patient patient)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      if (patient.Meta == null)
      {
        patient.Meta = new Meta();
      }

      if ((patient.Meta.Profile == null) || (patient.Meta.Profile.Count() == 0))
      {
        patient.Meta.Profile = new List<string>()
        {
          ProfileUrl,
        };

        return;
      }

      if (patient.Meta.Profile.Contains(ProfileUrl))
      {
        return;
      }

      patient.Meta.Profile = patient.Meta.Profile.Append(ProfileUrl);
    }

    /// <summary>
    /// Clear the assertion that a patient object conforms to the US Core Patient Profile.
    /// </summary>
    /// <param name="patient"></param>
    public static void UsCorePatientProfileClear(this Patient patient)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      if (patient.Meta == null)
      {
        return;
      }

      // set last updated so that meta is never empty
      patient.Meta.LastUpdated = DateTimeOffset.Now;

      if ((patient.Meta.Profile == null) || (patient.Meta.Profile.Count() == 0))
      {
        return;
      }

      if (patient.Meta.Profile.Contains(ProfileUrl))
      {
        int index = 0;
        foreach (string profile in patient.Meta.Profile)
        {
          if (profile.Equals(ProfileUrl, StringComparison.Ordinal))
          {
            break;
          }

          index++;
        }

        patient.Meta.ProfileElement.RemoveAt(index);
      }
    }
  }
}