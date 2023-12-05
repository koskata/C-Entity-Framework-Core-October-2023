namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;

    using Newtonsoft.Json;

    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var patientsDto = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Patient> patients = new List<Patient>();

            foreach (var patientDto in patientsDto)
            {
                if (!IsValid(patientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (patientDto.AgeGroup > 2 || patientDto.Gender > 1 || patientDto.AgeGroup < 0 || patientDto.Gender < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var patientToAdd = new Patient()
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup)patientDto.AgeGroup,
                    Gender = (Gender)patientDto.Gender
                };

                foreach (var id in patientDto.Medicines)
                {
                    if (patientToAdd.PatientsMedicines.Any(x => x.MedicineId == id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    patientToAdd.PatientsMedicines.Add(new PatientMedicine()
                    {
                        MedicineId = id,
                    });
                }

                patients.Add(patientToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedPatient, patientToAdd.FullName, patientToAdd.PatientsMedicines.Count));
            }

            context.Patients.AddRange(patients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }





        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportPharmacyDto[]), new XmlRootAttribute("Pharmacies"));

            var pharmaciesDto = (ImportPharmacyDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            List<Pharmacy> pharmacies = new List<Pharmacy>();

            foreach (var pharmacyDto in pharmaciesDto)
            {
                if (!IsValid(pharmacyDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (pharmacyDto.IsNonStop != "true" && pharmacyDto.IsNonStop != "false")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var pharmacyToAdd = new Pharmacy()
                {
                    IsNonStop = Convert.ToBoolean(pharmacyDto.IsNonStop), //
                    Name = pharmacyDto.Name,
                    PhoneNumber = pharmacyDto.PhoneNumber,
                };

                foreach (var medicine in pharmacyDto.Medicines)
                {
                    if (!IsValid(medicine))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!DateTime.TryParseExact(medicine.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!DateTime.TryParseExact(medicine.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (DateTime.ParseExact(medicine.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                    >= DateTime.ParseExact(medicine.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                    || medicine.Category > 4 || medicine.Category < 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var medicineToAdd = new Medicine()
                    {
                        Category = (Category)medicine.Category,
                        Name = medicine.Name,
                        Price = medicine.Price,
                        ProductionDate = DateTime.ParseExact(medicine.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(medicine.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Producer = medicine.Producer
                    };

                    if (pharmacyToAdd.Medicines.Any(x => x.Name == medicineToAdd.Name && x.Producer == medicineToAdd.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    pharmacyToAdd.Medicines.Add(medicineToAdd);
                }

                pharmacies.Add(pharmacyToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedPharmacy, pharmacyToAdd.Name, pharmacyToAdd.Medicines.Count));
            }

            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
