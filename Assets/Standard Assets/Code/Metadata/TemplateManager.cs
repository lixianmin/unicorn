﻿/********************************************************************
created:    2014-02-20
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using Unicorn;

namespace Metadata
{
    public class TemplateManager : IDisposable
    {
        public void Dispose()
        {
            Clear();
        }

        public bool AddTemplate(Template template)
        {
            if (null == template)
            {
                return false;
            }

            var type = template.GetType();
            var id = template.id;

            var table = FetchTemplateTable(type);
            if (table.TryGetValue(id, out var oldTemplate))
            {
                Logo.Error($"Duplicate template.id={id}, oldTemplate={oldTemplate}, newTemplate={template}");
                return false;
            }

            table.Add(template.id, template);
            return true;
        }

        public bool RemoveTemplate(Type templateType, int idTemplate)
        {
            if (null != templateType && _templateTables[templateType] is TemplateTable table)
            {
                var removed = table.Remove(idTemplate);
                if (removed && table.Count == 0)
                {
                    _templateTables.Remove(templateType);
                }

                return removed;
            }

            return false;
        }

        public Template GetTemplate(Type templateType, int idTemplate)
        {
            if (null != templateType && _templateTables[templateType] is TemplateTable table)
            {
                table.TryGetValue(idTemplate, out var template);
                return template;
            }

            return null;
        }

        internal TemplateTable FetchTemplateTable(Type templateType)
        {
            if (null != templateType)
            {
                if (_templateTables[templateType] is not TemplateTable table)
                {
                    table = new TemplateTable();
                    _templateTables.Add(templateType, table);
                }

                return table;
            }

            return null;
        }

        public TemplateTable GetTemplateTable(Type templateType)
        {
            if (null != templateType)
            {
                var table = _templateTables[templateType] as TemplateTable;
                return table;
            }

            return null;
        }

        public int GetTemplateCount()
        {
            var count = 0;
            var iter = _templateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var table = iter.Value as TemplateTable;
                count += table!.Count;
            }

            return count;
        }

        public void Clear()
        {
            _templateTables.Clear();
        }

        internal void TrimExcess()
        {
            var iter = _templateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var table = iter.Value as TemplateTable;
                table!.TrimExcess();
            }
        }

        public IEnumerable<KeyValuePair<Type, TemplateTable>> EnumerateTemplateTables()
        {
            var it = _templateTables.GetEnumerator();
            while (it.MoveNext())
            {
                var type = it.Key as Type;
                var table = it.Value as TemplateTable;
                var pair = new KeyValuePair<Type, TemplateTable>(type, table);

                yield return pair;
            }
        }

        public IEnumerable<Template> GetAllTemplates()
        {
            foreach (var pair in EnumerateTemplateTables())
            {
                var table = pair.Value;
                foreach (var template in table.Values)
                {
                    yield return template;
                }
            }
        }

        public override string ToString()
        {
            var sbText = new System.Text.StringBuilder();

            var iter = _templateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var type = iter.Key as Type;
                sbText.Append(type!.FullName);
                sbText.Append("---");

                var table = iter.Value as TemplateTable;
                sbText.Append(table!.Count.ToString());
                sbText.AppendLine();
            }

            var text = sbText.ToString();
            return text;
        }

        private readonly Hashtable _templateTables = new();
    }
}