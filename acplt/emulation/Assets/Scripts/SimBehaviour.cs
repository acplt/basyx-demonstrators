using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Xml;
using System.Linq;
using System;
using System.Xml;

public class SimBehaviour : MonoBehaviour
{
    public static Dictionary<string, SimBehaviour> behaviours;

    private List<Variable> variables;
    private static List<XmlElement> ioElements;
    private FieldInfo field;
    private object val;
    private static XmlDocument doc;
    private SimBehaviour behaviour;
    private string behaviourName;
    private int index;

    public static void Init(string config)
    {
        doc = new XmlDocument();
        doc.Load(config);
        ioElements = new List<XmlElement>();
        foreach (XmlElement ioElement in doc.DocumentElement)
        {
            ioElements.Add(ioElement);
        }

        behaviours = new Dictionary<string, SimBehaviour>();
        foreach (SimBehaviour behaviour in GameObject.FindObjectsOfType<SimBehaviour>())
        {
            behaviours.Add(behaviour.gameObject.name, behaviour);
            behaviour.GetVariables();
        }
    }

    public static SimBehaviour Find(string name)
    {
        return behaviours[name];
    }

    public void Refresh(SHM shm)
    {
        foreach (Variable variable in variables)
        {
            index = variable.Index * 4;
            if (variable.Direction == "FromSHM")
            {
                if (variable.DataType == "bool")
                {
                    variable.Field.SetValue(this, shm.GetBool(index));
                }
                else if (variable.DataType == "int")
                {
                    variable.Field.SetValue(this, shm.GetInt(index));
                }
                else if (variable.DataType == "float")
                {
                    variable.Field.SetValue(this, shm.GetFloat(index));
                }
            }
            else if (variable.Direction == "ToSHM")
            {
                if (variable.DataType == "bool")
                {
                    shm.SetBool(index, (bool)variable.Field.GetValue(this));
                }
                else if (variable.DataType == "int")
                {
                    shm.SetInt(index, (int)variable.Field.GetValue(this));
                }
                else if (variable.DataType == "float")
                {
                    shm.SetFloat(index, (float)variable.Field.GetValue(this));
                }
            }
        }
    }


    public Variable GetVariable(string variable)
    {
        try
        {
            return variables.Single(v => v.Name == variable);

        }
        catch (Exception ex)
        {
            return null;
        }
    }

        public void GetVariables()
    {
        variables = new List<Variable>();

        foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.FieldType == typeof(bool) || field.FieldType == typeof(float) || field.FieldType == typeof(int))
            {
                Variable variable = new Variable();
                variable.Field = field;
                variable.DataType = GetDataType(field);
                variable.Name = field.Name;
                try
                {
                    string name = gameObject.name + "." + variable.Name;
                    XmlElement ioElement = ioElements.Single(e => e.GetAttribute("name") == name);
                    variable.Index = int.Parse(ioElement.GetAttribute("signal"));
                    variable.Direction = ioElement.GetAttribute("direction");
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    variables.Add(variable);
                }
            }
        }
    }

    public Dictionary<string, string> GetFields()
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.FieldType == typeof(bool) || field.FieldType == typeof(float) || field.FieldType == typeof(int))
            {
                fields.Add(gameObject.name + "." + field.Name, GetDataType(field));
            }
        }
        return fields;
    }

    private string GetDataType(FieldInfo field)
    {
        if (field.FieldType.ToString() == "System.Boolean")
        {
            return "bool";
        }
        if (field.FieldType.ToString() == "System.Single")
        {
            return "float";
        }
        if (field.FieldType.ToString() == "System.Int32")
        {
            return "int";
        }
        return field.FieldType.ToString();
    }

    public string GetValue(string variable)
    {
        try
        {
            field = variables.Single(v => v.Name == variable).Field;
            return field.GetValue(this).ToString();
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public void SetValue(string variable, string value)
    {
        try
        {
            value = value.Replace(".", ",");
            field = variables.Single(v => v.Name == variable).Field;
            val = Convert.ChangeType(value, field.FieldType);
            field.SetValue(this, val);
        }
        catch (Exception ex)
        {

        }
    }
}

public class Variable
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public string Direction { get; set; }
    public int Index { get; set; }
    public FieldInfo Field { get; set; }

    public Variable()
    {
    }
}
