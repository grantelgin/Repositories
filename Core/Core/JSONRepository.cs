using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core
{
    public abstract class JSONRepository<TEntity> : IRepository<TEntity>
    {
        protected readonly List<TEntity> m_Records;
        protected readonly string msPath;

        protected JSONRepository(string sPath)
        {
            m_Records = new List<TEntity>();
            msPath = sPath;

            ReadFile_WithRetry();

        }


        protected virtual Predicate<TEntity> CreatePredicateFrom(Expression<Func<TEntity, bool>> expression)
        {
            return new Predicate<TEntity>(expression.Compile());
        }

        protected void ReadFile_WithRetry(int nRetries = 20, int nWaitMilliSeconds = 500)
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
                        throw new UnauthorizedAccessException("JSONRepo_0002 Repository file could not be accessed after 10 retries. " + msPath);
                    }
                }
            }
        }

        protected async Task WriteFile_WithRetry(int nRetries = 20, int nWaitMilliSeconds = 500)
        {
            Task t = Task.Run(() => {
                bool bFileWritten = false;

                while (bFileWritten == false)
                {
                    try
                    {
                        WriteFile();
                        bFileWritten = true;
                    }
                    catch
                    {
                        nRetries--;
                        System.Threading.Thread.Sleep(nWaitMilliSeconds);
                        if (nRetries < 0)
                        {
                            throw new UnauthorizedAccessException("JSONRepo_0001 Repository file could not be accessed after 10 retries. " + msPath);
                        }
                    }
                }
            }).ContinueWith(x => {
                if (x.IsCanceled || x.IsFaulted)
                {
                    throw x.Exception.Flatten();
                }
            });

            await t;
        }


        protected virtual void WriteFile()
        {
            string json = JsonConvert.SerializeObject(m_Records);

            using (StreamWriter writer = new StreamWriter(msPath, false))
            {
                writer.Write(json);
            }
        }

        protected virtual void ReadFile()
        {
            string contents = string.Empty;

            if (File.Exists(msPath))
            {
                contents = File.ReadAllText(msPath);
            }

            if (string.IsNullOrWhiteSpace(contents))
            {
                return;
            }

            List<TEntity> records = JsonConvert.DeserializeObject<List<TEntity>>(contents);

            m_Records.Clear();
            m_Records.AddRange(records);


        }

        public IEnumerable<TEntity> All()
        {
            return m_Records.ToArray();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            Func<TEntity, bool> func = predicate.Compile();
            return m_Records.Any(func);
        }

        public int Count
        {
            get { return m_Records.Count; }
        }



        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate<TEntity> pred = CreatePredicateFrom(predicate);
            return m_Records.RemoveAll(pred);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate<TEntity> pred = CreatePredicateFrom(predicate);
            return m_Records.Find(pred);
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate<TEntity> pred = CreatePredicateFrom(predicate);
            return m_Records.FindAll(pred);
        }

        public abstract int Update(TEntity t);

        public abstract List<TEntity> Create(List<TEntity> t);

        public abstract TEntity Create(TEntity t);

        public abstract int Delete(TEntity t);
    }
}
