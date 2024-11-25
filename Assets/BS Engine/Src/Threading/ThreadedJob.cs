///----------------------------------------------------------------------
/// @file ThreadedJob.cs
///
/// This file contains the declaration of ThreadedJob class.
/// 
/// A threaded job is a job that is executed in a different thread. It has all the properties of System.Thread
/// 
/// and can be used as corutine with WaitFor function.
/// 
/// Based on the following thread from Unity Forums:
/// 
/// http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
///
/// @author Alberto Martinez Villaran <tukaram92@gmail.com>
/// @date 27/11/2015
///----------------------------------------------------------------------



using UnityEngine;
using System.Collections;
using System.Threading;

namespace BSEngine
{
    namespace Threading
    {
        public abstract class ThreadedJob
        {

            #region Private params

            /// <summary>
            /// Flag to indicates when the thread is finished or not
            /// </summary>
            protected bool m_IsDone = false;

            /// <summary>
            /// Flag to indicate when the thread is aborted
            /// </summary>
            private bool m_Aborted = false;

            /// <summary>
            /// Object handler. Used to lock thread resources
            /// </summary>
            protected object m_Handle = new object();

            /// <summary>
            /// Thread reference
            /// </summary>
            protected Thread m_Thread = null;

            #endregion

            #region Public methods

            /// <summary>
            /// Property to acces check if the Thread is finished or not
            /// </summary>
            public bool IsDone
            {
                get
                {
                    bool tmp;
                    lock (m_Handle)
                    {
                        tmp = m_IsDone;
                    }

                    return tmp;
                }

                set
                {
                    lock (m_Handle)
                    {
                        m_IsDone = value;
                    }
                }
            }


            /// <summary>
            /// Property to acces check if the Thread is aborted or not
            /// </summary>
            public bool Aborted
            {
                get
                {
                    bool tmp;
                    lock (m_Handle)
                    {
                        tmp = m_Aborted;
                    }

                    return tmp;
                }

                set
                {
                    lock (m_Handle)
                    {
                        m_Aborted = value;
                    }
                }
            }


            /// <summary>
            /// Used to start the threading job
            /// </summary>
            public virtual void Start()
            {
                m_Thread = new Thread(Run);
                m_Thread.Start();
            }

            /// <summary>
            /// Used to abort the treading job
            /// </summary>
            public virtual void Abort()
            {
                if (m_Thread != null)
                {
                    m_Thread.Abort();
                }

                Aborted = true;
            }

            /// <summary>
            /// Used to update the job status
            /// </summary>
            /// <returns>Ture if the job has finished, False all the oteher cases</returns>
            public virtual bool Update()
            {
                if (IsDone)
                {
                    OnFinished();
                    return true;
                }
                else if (Aborted)
                {
                    OnAbort();
                    return true;
                }
                return false;
            }



            #endregion

            #region Private methods

            /// <summary>
            /// Main function of the threading job
            /// </summary>
            private void Run()
            {
                ThreadFunction();
                IsDone = true;
            }

            /// <summary>
            /// Corutine used to wait to the end of the of the job in other Corutines
            /// </summary>
            /// <returns></returns>
            public IEnumerator WaitFor()
            {
                while (!Update())
                {
                    yield return null;
                }
            }

            /// <summary>
            /// Threading function to do
            /// </summary>
            protected abstract void ThreadFunction();

            /// <summary>
            /// Tasks to do when the job has finished
            /// </summary>
            protected abstract void OnFinished();

            /// <summary>
            /// Tasks to do when the job is aborted
            /// </summary>
            protected abstract void OnAbort();

            #endregion

        }
    }
}
