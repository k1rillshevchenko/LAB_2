using System.Xml;
using System.Xml.Linq;
using LAB_2.Models; 

namespace LAB_2.Strategies;

public interface IXmlStrategy
{
    List<Student> Search(string xmlPath, SearchParams filter);
}

public class LinqStrategy : IXmlStrategy
{
    public List<Student> Search(string xmlPath, SearchParams filter)
    {
        var doc = XDocument.Load(xmlPath);
        return doc.Descendants("Student")
            .Where(s => (string.IsNullOrEmpty(filter.FullName) || s.Attribute("FullName")?.Value.Contains(filter.FullName) == true) &&
                        (string.IsNullOrEmpty(filter.Faculty) || s.Attribute("Faculty")?.Value == filter.Faculty))
            .Select(s => new Student
            {
                FullName = s.Attribute("FullName")?.Value ?? "",
                Faculty = s.Attribute("Faculty")?.Value ?? "",
                Department = s.Attribute("Department")?.Value ?? "",
                Course = s.Attribute("Course")?.Value ?? "",
                Address = s.Element("Residence")?.Attribute("Address")?.Value ?? ""
            }).ToList();
    }
}

public class DomStrategy : IXmlStrategy
{
    public List<Student> Search(string xmlPath, SearchParams filter)
    {
        var results = new List<Student>();
        var doc = new XmlDocument();
        doc.Load(xmlPath);
        var nodes = doc.SelectNodes("//Student");
        if (nodes == null) return results;

        foreach (XmlNode node in nodes)
        {
            var name = node.Attributes?["FullName"]?.Value;
            var fac = node.Attributes?["Faculty"]?.Value;

            if ((string.IsNullOrEmpty(filter.FullName) || (name?.Contains(filter.FullName) ?? false)) &&
                (string.IsNullOrEmpty(filter.Faculty) || fac == filter.Faculty))
            {
                results.Add(new Student
                {
                    FullName = name ?? "",
                    Faculty = fac ?? "",
                    Department = node.Attributes?["Department"]?.Value ?? "",
                    Course = node.Attributes?["Course"]?.Value ?? "",
                    Address = node.SelectSingleNode("Residence")?.Attributes?["Address"]?.Value ?? ""
                });
            }
        }
        return results;
    }
}

public class SaxStrategy : IXmlStrategy
{
    public List<Student> Search(string xmlPath, SearchParams filter)
    {
        var results = new List<Student>();
        using var reader = XmlReader.Create(xmlPath);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
            {
                var student = new Student
                {
                    FullName = reader.GetAttribute("FullName") ?? "",
                    Faculty = reader.GetAttribute("Faculty") ?? "",
                    Department = reader.GetAttribute("Department") ?? "",
                    Course = reader.GetAttribute("Course") ?? ""
                };

                while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student"))
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Residence")
                        student.Address = reader.GetAttribute("Address") ?? "";
                }

                if ((string.IsNullOrEmpty(filter.FullName) || student.FullName.Contains(filter.FullName)) &&
                    (string.IsNullOrEmpty(filter.Faculty) || student.Faculty == filter.Faculty))
                {
                    results.Add(student);
                }
            }
        }
        return results;
    }
}
