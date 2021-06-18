using System;
using System.Collections.Generic;
using System.IO;
using fhir_cs_profiling_basic.UsCore;
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
        /// <param name="patientJsonFilename">Output JSON file of our patient</param>
        /// <param name="outcomeJsonFilename">Output JSON file of our OperationOutcome</param>
        /// <param name="profileDirectory">Directory containing expanded profile packages</param>
        public static void Main(
          string patientJsonFilename = "",
          string outcomeJsonFilename = "",
          string profileDirectory = "")
        {
          string rootDir = Directory.GetCurrentDirectory();

          if (string.IsNullOrEmpty(patientJsonFilename))
          {
            patientJsonFilename = Path.Combine(rootDir, "patient.json");
          }

          if (string.IsNullOrEmpty(outcomeJsonFilename))
          {
            outcomeJsonFilename = Path.Combine(rootDir, "outcome.json");
          }

          if (string.IsNullOrEmpty(profileDirectory))
          {
            profileDirectory = Path.Combine(rootDir, "profiles");
          }

          // create a FHIR patient
          Patient patient = new Patient()
          {
            // US-Core requires an identifier
            Identifier = new List<Identifier>()
            {
              new Identifier("http://example.org/fhir/patient/identifier", "ABC123"),
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
              }
            },
            // US-Core requires a gender
            Gender = AdministrativeGender.Unknown,
          };

          // create a complex extension to add to our patient
          Extension raceExt = new Extension("http://hl7.org/fhir/us/core/StructureDefinition/us-core-race", null);
          raceExt.Extension = new List<Extension>()
          {
            new Extension("ombCategory", new Coding("urn:oid:2.16.840.1.113883.6.238", "1002-5", "American Indian or Alaska Native")),
            new Extension("text", new FhirString("Race default text"))
          };

          // add the extension to the patient
          patient.Extension.Add(raceExt);

          // set US Core Patient profile conformance
          patient.UsCorePatientProfileSet();
          
          // add a US Core Birthsex
          patient.UsCoreBirthsexSet(UsCoreBirthsex.UsCoreBirthsexValues.Female);

          if (patient.UsCoreBirthsexTryGet(out UsCoreBirthsex.UsCoreBirthsexValues? birthsex))
          {
            System.Console.WriteLine($"Found US Core Birthsex: {birthsex}");
          }
          else
          {
            System.Console.WriteLine("US Core Birthsex not found!");
          }

          // create a FHIR JSON serializer, using pretty-printing (nice formatting)
          FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings()
          {
            Pretty = true,
          });

          // serialize our patient into json
          string patientJson = serializer.SerializeToString(patient);

          // write the patient file
          File.WriteAllText(patientJsonFilename, patientJson);

          // display our patient on the console
          Console.WriteLine(patientJson);

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
          OperationOutcome outcome = validator.Validate(patient);

          // serialize our operation outcome to JSON
          string outcomeJson = serializer.SerializeToString(outcome);

          // write the operation outcome (for simplicity during recording)
          File.WriteAllText(outcomeJsonFilename, outcomeJson);

          // display our outcome on the console
          Console.WriteLine(outcomeJson);
        }
    }
}
