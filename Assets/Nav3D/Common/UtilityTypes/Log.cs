using System;
using System.Globalization;
using System.Text;

namespace Nav3D.Common
{
    public class Log
    {
        #region Attributes

        LimitedSizeQueue<string> m_Records;

        #endregion

        #region Contsructors

        public Log(int? _RecordsLimit)
        {
            m_Records = new LimitedSizeQueue<string>(_RecordsLimit.HasValue ? _RecordsLimit.Value : int.MaxValue);
        }

        #endregion

        #region Public methods

        public void Write(string _Record)
        {
            m_Records.Enqueue($"[{DateTime.Now.ToString("hh:mm:ss:fff", CultureInfo.InvariantCulture)}]: {_Record}");
        }

        public void WriteFormat(string _Record, params object[] _Args)
        {
            m_Records.Enqueue($"[{DateTime.Now.ToString("hh:mm:ss:fff", CultureInfo.InvariantCulture)}]: {string.Format(_Record, _Args)}");
        }

        public string GetText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string record in m_Records)
            {
                stringBuilder.AppendLine(record);
            }

            return stringBuilder.ToString();
        }

        public void Clear()
        {
            m_Records = new LimitedSizeQueue<string>(m_Records.Limit);
        }

        #endregion
    }
}