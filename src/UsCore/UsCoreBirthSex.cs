using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.UsCore
{
  /// <summary>
  /// Class with utility functions for Us Core Birth Sex operations.
  /// http://hl7.org/fhir/us/core/StructureDefinition-us-core-birthsex.html
  /// </summary>
  public static class UsCoreBirthSex
  {
    /// <summary>
    /// Official extension URL for the US Core Birth Sex extension
    /// </summary>
    public const string ExtensionUrl = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-birthsex";

    /// <summary>
    /// Codes for assigning sex at birth as specified by the Office of the National Coordinator for Health IT (ONC).
    /// http://hl7.org/fhir/us/core/ValueSet-birthsex.html
    /// </summary>
    public enum UsCoreBirthSexValues
    {
      /// <summary>F: Female</summary>
      Female,
      /// <summary>M: Male</summary>
      Male,
      /// <summary>UNK: Unkown - a proper value is applicable, but not known</summary>
      Unkown,
    }

    /// <summary>
    /// Set the US Core Birth Sex on a patient object.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="birthSex">Code to set, null to remove.</param>
    public static void UsCoreBirthSexSet(
      this Patient patient,
      UsCoreBirthSexValues? birthSex)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      switch (birthSex)
      {
        case UsCoreBirthSexValues.Female:
          patient.SetExtension(ExtensionUrl, new Code("F"));
          break;
        case UsCoreBirthSexValues.Male:
          patient.SetExtension(ExtensionUrl, new Code("M"));
          break;
        case UsCoreBirthSexValues.Unkown:
          patient.SetExtension(ExtensionUrl, new Code("UNK"));
          break;
        default:
          patient.RemoveExtension(ExtensionUrl);
          break;
      }
    }

    /// <summary>
    /// Gets a US Core Patient Birth Sex, null for not present.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="birthSex"></param>
    /// <returns></returns>
    public static bool UsCoreBirthSexTryGet(
      this Patient patient,
      out UsCoreBirthSexValues? birthSex)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      Extension ext = patient.GetExtension(ExtensionUrl);

      if ((ext == null) || (ext.Value == null))
      {
        birthSex = null;
        return false;
      }

      string value = ((Code)ext.Value).Value;

      switch (value)
      {
        case "F":
          birthSex = UsCoreBirthSexValues.Female;
          return true;

        case "M":
          birthSex = UsCoreBirthSexValues.Male;
          return true;

        case "UNK":
          birthSex = UsCoreBirthSexValues.Unkown;
          return true;
      }

      birthSex = null;
      return false;
    }
  }
}