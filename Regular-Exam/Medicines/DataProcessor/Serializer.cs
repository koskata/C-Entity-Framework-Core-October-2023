namespace Medicines.DataProcessor
{
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    using Medicines.Data;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;

    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {


            var patients = context.Patients
                            .Where(x => x.PatientsMedicines.Count >= 1 
                                && x.PatientsMedicines.Any(p => p.Medicine.ProductionDate > DateTime.Parse(date)))
                            .ToArray()
                            .Select(x => new ExportPatientDto
                            {
                                Gender = x.Gender.ToString().ToLower(),
                                FullName = x.FullName,
                                AgeGroup = x.AgeGroup.ToString(),
                                Medicines = x.PatientsMedicines
                                .Where(pm => pm.Medicine.ProductionDate > DateTime.Parse(date) && pm.Medicine.PatientsMedicines.Count >= 1)
                                .ToArray()
                                .OrderByDescending(x => x.Medicine.ExpiryDate).ThenBy(x => x.Medicine.Price)
                                .Select(p => new ExportManyMedicinesDto
                                {
                                    Category = p.Medicine.Category.ToString().ToLower(),
                                    Name = p.Medicine.Name,
                                    Price = $"{p.Medicine.Price:f2}",
                                    Producer = p.Medicine.Producer,
                                    ExpiryDate = p.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                                }).ToArray()
                            }).OrderByDescending(x => x.Medicines.Count()).ThenBy(x => x.FullName).ToArray();

            var serializer = new XmlSerializer(typeof(ExportPatientDto[]), new XmlRootAttribute("Patients"));

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, patients, namespaces);

            return sb.ToString().TrimEnd();
        }




        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {

            var medicines = context.Medicines
                                .Where(x => x.Category == (Category)medicineCategory && x.Pharmacy.IsNonStop == true)
                                .ToArray()
                                .Select(x => new ExportMedicinesDto
                                {
                                    Name = x.Name,
                                    Price = $"{x.Price:f2}",
                                    Pharmacy = new ExportOnePharmacyDto
                                    {
                                        Name = x.Pharmacy.Name,
                                        PhoneNumber = x.Pharmacy.PhoneNumber
                                    }
                                }).OrderBy(x => x.Price).ThenBy(x => x.Name).ToArray();

            return JsonConvert.SerializeObject(medicines, Formatting.Indented);
        }
    }
}
