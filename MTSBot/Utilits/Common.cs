using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MTSBot.Utilits
{
    public static class Tools
    {
        public static Boolean IsNullOrDefault<T>(this Nullable<T> value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static string EscapeXML(string unescaped)
        {
            return unescaped.Replace("&", "&#038;").
                Replace("\"", "&#034;").
                Replace("'", "&#039;").
                Replace("<", "&#060;").
                Replace(">", "&#062;").
                Replace("«", "&#171;").
                Replace("»", "&#187;");
        }

        public static string UnEscapeXML(string escaped)
        {
            return escaped.Replace("&quot;", "\"").
                Replace("&#034;", "\"").
                Replace("&#039;", "'").
                Replace("&#060;", "<").
                Replace("&#062;", ">").
                Replace("&#171;","\"").
                Replace("&#187;","\"");
        }
    }

    public static class ExceptionTools
    {
        /// <summary>
        /// Текст ошибки для Exception
        /// </summary>
        /// <param name="pEx"></param>
        /// <returns></returns>
        public static String GetExceptionText(Exception pEx)
        {
            if (pEx == null) return String.Empty;
            return String.Format(
                "Message: {0}\n" +
                "InnerException: {1}\n" +
                "TargetSite: {2}\n" +
                "StackTrace: {3}\n",
                pEx.Message,
                ((pEx.InnerException == null) ? "None" : pEx.InnerException.Message),
                pEx.TargetSite,
                pEx.StackTrace);
        }

        /// <summary>
        /// Текст ошибки для Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEx"></param>
        /// <returns></returns>
        public static String GetExceptionMessage<T>(this T pEx) where T : Exception
        {
            if (pEx == null) return String.Empty;
            else return String.Format(
                "Message: {0}\n" +
                "InnerException: {1}\n" +
                "TargetSite: {2}\n" +
                "StackTrace: {3}\n",
                pEx.Message,
                ((pEx.InnerException == null) ? "None" : pEx.InnerException.Message),
                pEx.TargetSite,
                pEx.StackTrace);
        }

        /// <summary>
        /// Генерация описания ошибки для SqlException
        /// </summary>
        /// <param name="sqlEx"></param>
        /// <returns></returns>
        public static String GetSqlExceptionText(SqlException pSqlEx)
        {

            if (pSqlEx == null) return String.Empty;
            return String.Format(
                "SqlError: {0}\n" +
                "LineNumber: {1}\n" +
                "{2}",
                pSqlEx.Number,
                pSqlEx.LineNumber,
                // GetExceptionText(pSqlEx)
                pSqlEx.GetExceptionMessage()
                );
        }

        /// <summary>
        /// Генерация описания ошибки для WebException
        /// </summary>
        /// <param name="pWebEx"></param>
        /// <returns></returns>
        public static String GetWebExceptionText(WebException pWebEx)
        {
            HttpStatusCode httpCode;
            String responseText = String.Empty;
            HttpWebResponse webResponse = (HttpWebResponse)pWebEx.Response;
            if (webResponse == null)
            {
                httpCode = HttpStatusCode.InternalServerError;
                responseText = HttpStatusCode.InternalServerError.ToString();
            }
            else
            {
                httpCode = webResponse.StatusCode;
                // using (Stream stream = webResponse.GetResponseStream())
                Stream stream = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    responseText = reader.ReadToEnd(); // Читаем текст ответа от веб-сервиса в response
                }
                // stream.Close();
            }
            return String.Format(
                "HttpStatusCode: {0}\n" +
                "HttpResponseText: {1}\n" +
                "{2}",
                (int)httpCode,
                responseText,
                // GetExceptionText(pWebEx)
                pWebEx.GetExceptionMessage()
                );
        }
    }
}