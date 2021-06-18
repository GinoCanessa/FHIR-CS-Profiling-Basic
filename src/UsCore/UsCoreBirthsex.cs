using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.UsCore
{
  /// <summary>
  /// Class with Patient extensions for the US Core Birthsex extension
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-birthsex.html
  /// </summary>
  public static class UsCoreBirthsex
  {
    /// <summary>
    /// The official URL for the US Core Birthsex extension.
    /// </summary>
    public const string ExtensionUrl = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-birthsex";

    /// <summary>
    /// Codes for assigning sex at birth as specified by the Office of the National Coordinator for Health IT (ONC).
    /// http://hl7.org/fhir/us/core/ValueSet-birthsex.html
    /// </summary>
    public enum UsCoreBirthsexValues
    {
      /// <summary>'F': Female</summary>
      Female,

      /// <summary>'M': Male</summary>
      Male,

      /// <summary>'UNK': Unkown - A proper value is applicable, but not known.</summary>
      Unknown,
    }

    /// <summary>
    /// Set the US Core Birthsex on a patient object, null to remove
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="birthsex"></param>
    public static void UsCoreBirthsexSet(
      this Patient patient,
      UsCoreBirthsexValues birthsex)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      switch (birthsex)
      {
        case UsCoreBirthsexValues.Female:
          patient.SetExtension(ExtensionUrl, new Code("F"));
          break;

        case UsCoreBirthsexValues.Male:
          patient.SetExtension(ExtensionUrl, new Code("M"));
          break;

        case UsCoreBirthsexValues.Unknown:
          patient.SetExtension(ExtensionUrl, new Code("UNK"));
          break;
      }
    }

    /// <summary>
    /// Try to get the US Core Birthsex from a patient object
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="birthsex"></param>
    /// <returns>True if value was read, false otherwise.</returns>
    public static bool UsCoreBirthsexTryGet(this Patient patient, out UsCoreBirthsexValues? birthsex)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      Extension ext = patient.GetExtension(ExtensionUrl);

      if ((ext == null) || (ext.Value == null))
      {
        birthsex = null;
        return false;
      }

      string value = ((Code)ext.Value).Value;

      switch (value)
      {
        case "F":
          birthsex = UsCoreBirthsexValues.Female;
          break;

        case "M":
          birthsex = UsCoreBirthsexValues.Male;
          break;

        case "UNK":
          birthsex = UsCoreBirthsexValues.Unknown;
          break;

        default:
          birthsex = null;
          return false;
      }

      return true;
    }

    /// <summary>
    /// Clear any existing US Core Birthsex extensions from a patient
    /// </summary>
    /// <param name="patient"></param>
    public static void UsCoreBirthsexClear(this Patient patient)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      patient.RemoveExtension(ExtensionUrl);
    }
  }
}