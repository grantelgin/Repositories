using Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// Repository Implementation for Delimiter Separated Values (most commonly Comma or Tab Separated Values)
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DSVRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        
        protected readonly List<TEntity> m_Records;
        protected readonly string msPath;
        protected readonly char mDelimiter;
        private Type mTypeTEntity;
        BindingFlags mFlags;

        public int Count => throw new NotImplementedException();

        protected DSVRepository(char delimiter, string sPath, BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
        {
            if (string.IsNullOrEmpty(sPath))
            {
                throw new ArgumentNullException(nameof(sPath));
            }
            m_Records = new List<TEntity>();
            
            mDelimiter = delimiter;
            msPath = sPath;
            mTypeTEntity = typeof(TEntity);
            mFlags = bindingFlags;

            ReadFile_WithRetry();
        }

        private void ReadFile_WithRetry(int nRetries = 20, int nWaitMilliSeconds = 500)
        {
            bool bFileRead = false;

            while (bFileRead == false)
            {
                try
                {
                    ReadFile();
                    bFileRead = true;
                }
                catch
                {
                    nRetries--;
                    System.Threading.Thread.Sleep(nWaitMilliSeconds);
                    if (nRetries < 0)
                    {
                        throw new UnauthorizedAccessException("DSVRepo_0002 Repository file could not be accessed after 10 retries. " + msPath);
                    }
                }
            }
        }

        private void ReadFile()
        {
            string sContents = string.Empty;
            if (File.Exists(msPath))
            {
                sContents = File.ReadAllText(msPath);
            }

            if (string.IsNullOrWhiteSpace(sContents))
            {
                return;
            }

            using(StringReader sr  = new StringReader(sContents))
            {
                string sLine = sr.ReadLine();
                string[] header = sLine.Split(mDelimiter);

                Dictionary<string, int> columnPositions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < header.Length; i++)
                {
                    columnPositions.Add(header[i], i);
                }

                while ((sLine = sr.ReadLine()) != null)
                {
                    string[] row = sLine.Split(mDelimiter);
                    TEntity obj = new TEntity();
                    FieldInfo[] fields = obj.GetType().GetFields(mFlags);
                    PropertyInfo[] props = obj.GetType().GetProperties(mFlags);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (columnPositions.TryGetValue(fields[i].Name, out int nColPosition))
                        {
                            //---TODO! Need a field data type converter method. 
                            fields[i].SetValue(obj, row[nColPosition]);
                        }
                    }

                    for (int i = 0; i < props.Length; i++)
                    {
                        if (columnPositions.TryGetValue(props[i].Name, out int nColPosition))
                        {
                            //---TODO! Need a field data type converter method. 
                            props[i].SetValue(obj, row[nColPosition]);
                        }
                    }
                }

            }
        }

        public IEnumerable<TEntity> All()
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Create(TEntity t)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> Create(List<TEntity> t)
        {
            throw new NotImplementedException();
        }

        public int Delete(TEntity t)
        {
            throw new NotImplementedException();
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Update(TEntity t)
        {
            throw new NotImplementedException();
        }
    }
}
