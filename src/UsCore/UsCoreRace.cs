using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace fhir_cs_profiling_basic.UsCore
{
  /// <summary>
  /// Class with Patient extensions for the US Core Race extension
  /// http://hl7.org/fhir/us/core/StructureDefinition/us-core-race.html
  /// </summary>
  public static class UsCoreRace
  {
    /// <summary>
    /// The official URL for the US Core Race extension.
    /// </summary>
    public const string ExtensionUrl = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-race";

    /// <summary>
    /// US Core Race extension, sub extension OMB category field
    /// </summary>
    public const string UrlOmbCategory = "ombCategory";

    /// <summary>
    /// US Core Race extension, sub extension detailed field
    /// </summary>
    public const string UrlDetailed = "detailed";

    /// <summary>
    /// US Core Race extension, sub extension text field
    /// </summary>
    public const string UrlText = "text";

    /// <summary>
    /// System URL for CDC Race and Ethnicity
    /// http://hl7.org/fhir/us/core/CodeSystem-cdcrec.html
    /// </summary>
    public const string SystemCdcRec = "urn:oid:2.16.840.1.113883.6.238";

    /// <summary>
    /// System URL for HL7 Null Flavors
    /// https://terminology.hl7.org/2.1.0/CodeSystem-v3-NullFlavor.html
    /// </summary>
    public const string SystemNullFlavor = "http://terminology.hl7.org/CodeSystem/v3-NullFlavor";

    /// <summary>
    /// OMB Race Categories
    /// http://hl7.org/fhir/us/core/ValueSet-omb-race-category.html
    /// </summary>
    public enum UsCoreOmbRaceCategoryValues
    {
      /// <summary>
      ///       Code: 1002-5
      ///     System: urn:oid:2.16.840.1.113883.6.238
      /// Definition: American Indian or Alaska Native
      /// </summary>
      AmericanIndianOrAlaskaNative,

      /// <summary>
      ///       Code: 2028-9
      ///     System: urn:oid:2.16.840.1.113883.6.238
      /// Definition: Asian
      /// </summary>
      Asian,

      /// <summary>
      ///       Code: 2054-5
      ///     System: urn:oid:2.16.840.1.113883.6.238
      /// Definition: Black or African American
      /// </summary>
      BlackOrAfricanAmerican,

      /// <summary>
      ///       Code: 2067-8
      ///     System: urn:oid:2.16.840.1.113883.6.238
      /// Definition: Native Hawaiian or Other Pacific Islander
      /// </summary>
      NativeHawaiianOrOtherPacificIslander,

      /// <summary>
      ///       Code: 2106-3
      ///     System: urn:oid:2.16.840.1.113883.6.238
      /// Definition: White
      /// </summary>
      White,

      /// <summary>
      ///       Code: UNK
      ///     System: http://terminology.hl7.org/CodeSystem/v3-NullFlavor
      /// Definition: A proper value is applicable, but not known.
      /// </summary>
      Unknown,

      /// <summary>
      ///       Code: ASKU
      ///     System: http://terminology.hl7.org/CodeSystem/v3-NullFlavor
      /// Definition: Information was sought but not found (e.g., patient was asked but didn't know).
      /// </summary>
      AskedButNoAnswer,
    }

    /// <summary>
    /// Convert OMB race category enum values to codings
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static Coding UsCoreOmbRaceCategoryToCoding(UsCoreOmbRaceCategoryValues value)
    {
      switch (value)
      {
        case UsCoreOmbRaceCategoryValues.AmericanIndianOrAlaskaNative:
          return new Coding(SystemCdcRec, "1002-5", "American Indian or Alaska Native");

        case UsCoreOmbRaceCategoryValues.Asian:
          return new Coding(SystemCdcRec, "2028-9", "Asian");

        case UsCoreOmbRaceCategoryValues.BlackOrAfricanAmerican:
          return new Coding(SystemCdcRec, "2054-5", "Black or African American");

        case UsCoreOmbRaceCategoryValues.NativeHawaiianOrOtherPacificIslander:
          return new Coding(SystemCdcRec, "2076-8", "Native Hawaiian or Other Pacific Islander");

        case UsCoreOmbRaceCategoryValues.White:
          return new Coding(SystemCdcRec, "2106-3", "White");

        case UsCoreOmbRaceCategoryValues.Unknown:
          return new Coding(SystemNullFlavor, "UNK", "Unknown");

        case UsCoreOmbRaceCategoryValues.AskedButNoAnswer:
          return new Coding(SystemNullFlavor, "ASKU", "Asked but no answer");
      }

      throw new ArgumentException($"Invalid or unhandled UsCoreOmbRaceCategory: {value}");
    }

    /// <summary>
    /// Add or replace a US Core Race extension, with the specified values.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="text"></param>
    /// <param name="ombCategory"></param>
    /// <param name="detailed"></param>
    public static void UsCoreRaceSet(
      this Patient patient,
      string text,
      IEnumerable<UsCoreOmbRaceCategoryValues> ombCategory = null,
      IEnumerable<Coding> detailed = null)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      if (string.IsNullOrEmpty(text))
      {
        throw new ArgumentNullException(nameof(text));
      }

      // create our parent US Core Race extension object (null because we will have extensions, not a value)
      Extension usCoreRaceExt = new Extension(ExtensionUrl, null);

      // set text - good to replace since it is mandatory 1..1
      usCoreRaceExt.SetExtension(UrlText, new FhirString(text));

      // add in any OMB categories
      if ((ombCategory != null) && (ombCategory.Any()))
      {
        foreach (UsCoreOmbRaceCategoryValues enumValue in ombCategory)
        {
          usCoreRaceExt.AddExtension(UrlOmbCategory, UsCoreOmbRaceCategoryToCoding(enumValue));
        }
      }

      // add in any detailed codings
      if ((detailed != null) && (detailed.Any()))
      {
        foreach (Coding coding in detailed)
        {
          usCoreRaceExt.AddExtension(UrlDetailed, coding);
        }
      }

      patient.RemoveExtension(ExtensionUrl);
      patient.Extension.Add(usCoreRaceExt);
    }

    /// <summary>
    /// Add an OMB Category to a US Core race extension
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="ombCategory"></param>
    public static void UsCoreRaceOmbCategoryAdd(this Patient patient, UsCoreOmbRaceCategoryValues ombCategory)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      Coding ombCategoryCoding = UsCoreOmbRaceCategoryToCoding(ombCategory);

      // get the parent US Core Race extension object
      Extension usCoreRaceExt = patient.GetExtension(ExtensionUrl);

      if (usCoreRaceExt == null)
      {
        UsCoreRaceSet(
          patient,
          "Generated Text",
          new UsCoreOmbRaceCategoryValues[] { ombCategory });

        return;
      }

      // get the list of existing OMB Categories
      IEnumerable<Extension> existingOmbCategories = usCoreRaceExt.GetExtensions(UrlOmbCategory);

      // create a list of OMB Categories to set with
      List<Extension> ombCategories = new List<Extension>();

      // traverse the existing categories
      foreach (Extension ext in existingOmbCategories)
      {
        // check to see if our value would be a duplicate
        if (((Coding)(ext.Value)).Code == ombCategoryCoding.Code)
        {
          // already exists, nothing else to do
          return;
        }

        // check for categories we should remove
        switch (((Coding)(ext.Value)).Code)
        {
          case "UNK":
          case "ASKU":
            // ignore
            break;
          
          default:
            ombCategories.Add(ext);
            break;
        }
      }

      if ((ombCategories.Count > 0) &&
          ((ombCategory == UsCoreOmbRaceCategoryValues.Unknown) || (ombCategory == UsCoreOmbRaceCategoryValues.AskedButNoAnswer)))
      {
        // don't add unknown if we already have a value
        return;
      }

      // remove ALL OMB Categories
      usCoreRaceExt.RemoveExtension(UrlOmbCategory);

      // add our replacement OMB categories
      foreach (Extension ext in ombCategories)
      {
        usCoreRaceExt.Extension.Add(ext);
      }

      // add the requested category
      usCoreRaceExt.AddExtension(UrlOmbCategory, ombCategoryCoding);
    }

    /// <summary>
    /// Set the text on an existing US Core Race extension, or adds a new US Core Race extension
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="text"></param>
    public static void UsCoreRaceTextSet(this Patient patient, string text)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      if (string.IsNullOrEmpty(text))
      {
        throw new ArgumentNullException(nameof(text));
      }

      Extension usCoreRaceExt = patient.GetExtension(ExtensionUrl);

      if (usCoreRaceExt == null)
      {
        UsCoreRaceSet(patient, text);
        return;
      }

      usCoreRaceExt.SetExtension(UrlText, new FhirString(text));
    }

    /// <summary>
    /// Try to read the text value of a US Core Race extension
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool UsCoreRaceTextTryGet(this Patient patient, out string text)
    {
      if (patient == null)
      {
        text = null;
        return false;
      }

      Extension usCoreRaceExt = patient.GetExtension(ExtensionUrl);

      if (usCoreRaceExt == null)
      {
        text = null;
        return false;
      }

      FhirString fhirText = usCoreRaceExt.GetExtensionValue<FhirString>(UrlText);

      if (fhirText != null)
      {
        text = fhirText.Value;
        return true;
      }

      text = null;
      return false;
    }

    /// <summary>
    /// Clear any existing US Core Race extensions from a patient
    /// </summary>
    /// <param name="patient"></param>
    public static void UsCoreRaceClear(this Patient patient)
    {
      if (patient == null)
      {
        throw new ArgumentNullException(nameof(patient));
      }

      patient.RemoveExtension(ExtensionUrl);
    }
  }
}