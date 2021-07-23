using System;
using System.Collections.Generic;
using System.IO;
using fhir_cs_profiling_basic.UsCore;
using fhir_cs_profiling_basic.Validation;
using FluentValidation.Results;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;

namespace fhir_cs_profiling_basic
{
  /// <summary>
  /// Program to test working with IGs/Profiles
  /// </summary>
  public static class Program
  {
    /// <summary>
    /// Program to test working with IGs/Profiles
    /// </summary>
    /// <param name="resourceJsonFilename">Output JSON file of our resource</param>
    /// <param name="outcomeJsonFilename">Output JSON file of our OperationOutcome</param>
    /// <param name="profileDirectory">Directory containing expanded profile packages</param>
    public static void Main(
      string resourceJsonFilename = "",
      string outcomeJsonFilename = "",
      string profileDirectory = "")
    {
      string rootDir = Directory.GetCurrentDirectory();

      if (string.IsNullOrEmpty(resourceJsonFilename))
      {
        resourceJsonFilename = Path.Combine(rootDir, "resource.json");
      }

      if (string.IsNullOrEmpty(outcomeJsonFilename))
      {
        outcomeJsonFilename = Path.Combine(rootDir, "outcome.json");
      }

      if (string.IsNullOrEmpty(profileDirectory))
      {
        profileDirectory = Path.Combine(rootDir, "profiles");
      }

      // Patient patient = CreatePatient();
      // ValidateResource<Patient>(patient, new UsCorePatientValidator());

      Observation resource = CreateObservation();
      ValidateResource<Observation>(resource, new UsCoreBloodPressureValidator());

      // ValidateOfficial(resource, resourceJsonFilename, profileDirectory, outcomeJsonFilename);
    }

    /// <summary>
    /// Validate a resource against a fluent validator
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="validator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static bool ValidateResource<T>(
      T resource, 
      FluentValidation.AbstractValidator<T> validator)
    {
      ValidationResult validationResult = validator.Validate(resource);

      System.Console.WriteLine($"Validation passed: {validationResult.IsValid}");

      if (!validationResult.IsValid)
      {
        foreach (ValidationFailure failure in validationResult.Errors)
        {
          System.Console.WriteLine($"Validation Issue: {failure.PropertyName}: {failure.ErrorMessage}");
        }
      }

      return validationResult.IsValid;
    }

    /// <summary>
    /// Go through official FHIR validation
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="resourceJsonFilename"></param>
    /// <param name="profileDirectory"></param>
    /// <param name="outcomeJsonFilename"></param>
    private static void ValidateOfficial(
      Resource resource,
      string resourceJsonFilename,
      string profileDirectory,
      string outcomeJsonFilename)
    {
      // create a FHIR JSON serializer, using pretty-printing (nice formatting)
      FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
      {
        Pretty = true,
      });

      // serialize our patient into json
      string resourceJson = serializer.SerializeToString(resource);

      // write the patient file
      File.WriteAllText(resourceJsonFilename, resourceJson);

      // display our patient on the console
      //Console.WriteLine(resourceJson);

      // create a cached resolver for resource validation
      IResourceResolver resolver = new CachedResolver(
        // create a multi-resolver, which can resolve resources from more than one source
        new MultiResolver(
          // create the default FHIR specification resolver (specification.zip included in HL7.fhir.specification.r4)
          ZipSource.CreateValidationSource(),
          // create the directory source resolver, which points to our profiles directory
          new DirectorySource(profileDirectory, new DirectorySourceSettings()
          {
            IncludeSubDirectories = true,
          })
        )
      );

      // create a resource validator, which uses our cached resolver
      Validator validator = new Validator(new ValidationSettings()
      {
        ResourceResolver = resolver,
      });

      // validate our patient and save the operation outcome
      OperationOutcome outcome = validator.Validate(resource);

      // serialize our operation outcome to JSON
      string outcomeJson = serializer.SerializeToString(outcome);

      // write the operation outcome (for simplicity during recording)
      File.WriteAllText(outcomeJsonFilename, outcomeJson);

      // display our outcome on the console
      //Console.WriteLine(outcomeJson);
    }

    /// <summary>
    /// Create a US Core Blood Pressure observation
    /// </summary>
    /// <returns></returns>
    private static Observation CreateObservation()
    {
      // Observation resource = new Observation()
      // {
      //   Status = ObservationStatus.Unknown,
      //   Subject = new ResourceReference("Patient/test"),
      //   Effective = new FhirDateTime(2021, 07, 02, 10, 0, 0, new TimeSpan()),
      // };

      // resource.UsCoreVitalSignsProfileSet();
      // resource.UsCoreVitalSignsCategorySet();

      // resource.UsCoreBloodPressureProfileSet();
      // resource.UsCoreBloodPressureCodeSet();
      // resource.UsCoreBloodPressureSystolicSet(100);
      // resource.UsCoreBloodPressureDiastolicSet(70);

      Observation resource = UsCoreBloodPressure.Create(
        ObservationStatus.Final,
        new ResourceReference("Patient/factory"),
        new FhirDateTime(2021, 07, 02, 10, 0, 0, new TimeSpan()),
        100,
        70);

      return resource;
    }

    /// <summary>
    /// Create a US Core Patient
    /// </summary>
    /// <returns></returns>
    private static Patient CreatePatient()
    {
      // create a FHIR patient
      Patient resource = new Patient()
      {
        // US-Core requires an identifier
        Identifier = new List<Identifier>()
        {
          new Identifier("http://example.org/fhir/patient/identifier", "ABC123"),
          //new Identifier(null, "ABC123"),
        },
        // US-Core requires a patient name with a: Given, Family, or Data Absent Reason (DAR)
        Name = new List<HumanName>()
        {
          new HumanName()
          {
            Given = new List<string>()
            {
              "Test",
            }
          },
          // new HumanName(),
        },
        // US-Core requires a gender
        Gender = AdministrativeGender.Unknown,
      };

      // set US Core Patient profile conformance
      resource.UsCorePatientProfileSet();
      
      // add a US Core Birthsex
      resource.UsCoreBirthsexSet(UsCoreBirthsex.UsCoreBirthsexValues.Female);

      // if (resource.UsCoreBirthsexTryGet(out UsCoreBirthsex.UsCoreBirthsexValues? birthsex))
      // {
      //   System.Console.WriteLine($"Found US Core Birthsex: {birthsex}");
      // }
      // else
      // {
      //   System.Console.WriteLine("US Core Birthsex not found!");
      // }

      resource.UsCoreRaceSet(
        "Race default text", 
        new UsCoreRace.UsCoreOmbRaceCategoryValues[] { UsCoreRace.UsCoreOmbRaceCategoryValues.Unknown });

      resource.UsCoreRaceTextSet("Updated text");

      resource.UsCoreRaceOmbCategoryAdd(UsCoreRace.UsCoreOmbRaceCategoryValues.AmericanIndianOrAlaskaNative);
      resource.UsCoreRaceOmbCategoryAdd(UsCoreRace.UsCoreOmbRaceCategoryValues.AmericanIndianOrAlaskaNative);
      resource.UsCoreRaceOmbCategoryAdd(UsCoreRace.UsCoreOmbRaceCategoryValues.Asian);
      resource.UsCoreRaceOmbCategoryAdd(UsCoreRace.UsCoreOmbRaceCategoryValues.Unknown);

      return resource;
    }
  }
}
