using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        private List<string> tasks = new List<string>();

        public Form1()
        {
            InitializeComponent();
            dropPriority.DataSource = Enum.GetValues(typeof(Task.PriorityType)); // Istället för att göra det menuellt och hålla på med strings
            listBoxTasks.DataSource = tasks;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Skapa ny instans av klassen Task
            Task task = new Task()
            {
                DueDate = dateTimePicker.Value.ToShortDateString(),
                Priority = (Task.PriorityType)dropPriority.SelectedValue,
                Description = txtDescription.Text
            };

            // Lägg till i lista
            tasks.Add(task.ToString());

            // Validera...
            if (Validate(task))
            {
                listBoxTasks.DataSource = null; // Rensa listbox
                listBoxTasks.DataSource = tasks; // Lägg till ny task i listbox
            }
        }

        private bool Validate (Task task)
        {
            bool validDate = DateTime.TryParse(task.DueDate, out _); // Validera datum
            bool validPriority = Enum.IsDefined(typeof(Task.PriorityType), dropPriority.SelectedValue); // Validera prioritet med Enum.IsDefined

            if (validDate && validPriority)
            {
                return true; // Om OK
            }

            else
            {
                MessageBox.Show("Validation failed, please check values and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        // Spara task list till txt fil...
        private void toolStripSaveText_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileText.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = File.CreateText(saveFileText.FileName))
                    {
                        foreach (string s in tasks)
                        {
                            writer.WriteLine(s);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Spara task list till XML fil som sedan kan laddas upp igen!
        private void toolStripSaveXML_Click(object sender, EventArgs e)
        {
            Serializer();
        }

        // XML logik, svårt att förklara med kommentarer, hitta mig i skolan om förklaring önskas
        private void Serializer()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<string>));
                XmlDocument doc = new XmlDocument();

                using (StringWriter writer = new StringWriter())
                {
                    ser.Serialize(writer, tasks);
                    string serXML = writer.ToString();

                    XmlDocument serDoc = new XmlDocument();
                    serDoc.LoadXml(serXML);
                    doc.AppendChild(doc.ImportNode(serDoc.DocumentElement, true));
                }

                if (saveFileXML.ShowDialog() == DialogResult.OK)
                {
                    doc.Save(saveFileXML.FileName);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 

        // XML laddnings logik, också svårt att förklara
        private void toolStripLoadXML_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFile.FileName;

                    XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                    List<string> deserializedTasks;

                    using (Stream reader = new FileStream(fileName, FileMode.Open))
                    {
                        Console.WriteLine(fileName);
                        deserializedTasks = (List<string>)serializer.Deserialize(reader);
                    }

                    foreach (string s in deserializedTasks)
                    {
                        tasks.Add(s);
                    }

                    listBoxTasks.DataSource = null;
                    listBoxTasks.DataSource = tasks;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hjälp knapp
        private void toolHelp_Click(object sender, EventArgs e)
        {
            string description = $"Welcome to the Task Manager!\n \nUse the provided tools to create reminders for yourself and save them to the program or to a separate file!";
            MessageBox.Show(description);
        }

    }
}
