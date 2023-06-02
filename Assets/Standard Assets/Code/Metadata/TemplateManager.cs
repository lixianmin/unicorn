
/********************************************************************
created:    2014-02-20
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Unicorn;
using Unicorn.Collections;

namespace Metadata
{
	public class TemplateManager: IDisposable
    {
        public void Dispose ()
        {
            Clear();
        }

        public bool AddTemplate (Template template)
        {
            if (null == template)
            {
                return false;
            }

			var table = FetchTemplateTable(template.GetType());

            var oldTemplate = table.GetEx(template.id);
            if (null != oldTemplate)
            {
                Logo.Error("Duplicate template.id ={0}, oldTemplate={1}, newTemplate={2}"
                                        , template.id, oldTemplate, template);
                return false;
            }

            table.Add(template.id, template);
            return true;
        }

		public bool RemoveTemplate (Type templateType, int idTemplate)
		{
			if (null != templateType)
			{
                var table = _mTemplateTables[templateType] as TemplateTable;
				
				if (null != table)
				{
					var removed = table.Remove(idTemplate);
					if (removed && table.Count == 0)
					{
                        _mTemplateTables.Remove(templateType);
					}

					return removed;
				}
			}
			
			return false;
		}

		public Template GetTemplate (Type templateType, int idTemplate)
		{
			if (null != templateType)
			{
                var table = _mTemplateTables[templateType] as TemplateTable;
				
				if (null != table)
				{
					Template template;
					table.TryGetValue(idTemplate, out template);
					return template;
				}
			}

			return null;
		}

		internal TemplateTable FetchTemplateTable (Type templateType)
		{
			if (null != templateType)
			{
                var table = _mTemplateTables[templateType] as TemplateTable;
                if (null == table)
                {
                    table = new TemplateTable();
                    _mTemplateTables.Add(templateType, table);
                }

				return table;
			}

			return null;
		}

		public TemplateTable GetTemplateTable (Type templateType)
		{
			if (null != templateType)
			{
                var table = _mTemplateTables[templateType] as TemplateTable;
				return table;
			}

			return null;
		}
        
        public int GetTemplateCount ()
        {
            var count = 0;
            var iter = _mTemplateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var table = iter.Value as TemplateTable;
                count += table.Count;
            }
            
            return count;
        }

        public void Clear ()
        {
            _mTemplateTables.Clear();
        }

		internal void TrimExcess ()
        {
            var iter = _mTemplateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var table = iter.Value as TemplateTable;
                table.TrimExcess();
            }
        }

        internal IEnumerable<KeyValuePair<Type, TemplateTable>> EnumerateTemplateTables ()
        {
            var iter = _mTemplateTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var type = iter.Key as Type;
                var table = iter.Value as TemplateTable;
                var pair = new KeyValuePair<Type, TemplateTable>(type, table);

                yield return pair;
            }
        }

		public IEnumerable<Template> GetAllTemplates ()
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

		public override string ToString ()
		{
			var sbText = new System.Text.StringBuilder();

            var iter = _mTemplateTables.GetEnumerator();
            while (iter.MoveNext())
			{
                var type = iter.Key as Type;
                sbText.Append(type.FullName);
				sbText.Append("---");

                var table = iter.Value as TemplateTable;
				sbText.Append(table.Count.ToString());
				sbText.AppendLine();
			}

			var text = sbText.ToString();
			return text;
		}

        private readonly Hashtable _mTemplateTables = new Hashtable();
    }
}