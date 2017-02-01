using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WordScraper.PhantomJS
{
    /// <summary>PhantomJS Wrapper</summary>
    public class PhantomJS
    {
        private List<string> errorLines = new List<string>();
        private Process PhantomJsProcess;

        /// <summary>Get or set path where phantomjs.exe is located</summary>
        /// <remarks>
        /// By default this property initialized with assembly location (app bin folder).
        /// This is exact place where phantomjs.exe is copied by PhantomJS nuget package.
        /// </remarks>
        public string ToolPath { get; set; }

        /// <summary>
        /// Get or set location for temp files (default location: <see cref="M:System.IO.Path.GetTempPath" />)
        /// </summary>
        /// <remarks>
        /// Temp file is created in <see cref="M:NReco.PhantomJS.PhantomJS.RunScript(System.String,System.String[])" /> and <see cref="M:NReco.PhantomJS.PhantomJS.RunScriptAsync(System.String,System.String[])" /> methods
        /// (PhantomJS can read javascript code only from file).</remarks>
        public string TempFilesPath { get; set; }

        /// <summary>
        /// Get or set PhantomJS tool executive file name ('phantomjs.exe' by default)
        /// </summary>
        public string PhantomJsExeName { get; set; }

        /// <summary>Get or set extra PhantomJS switches/options</summary>
        public string CustomArgs { get; set; }

        /// <summary>
        /// Get or set PhantomJS process priority (Normal by default)
        /// </summary>
        public ProcessPriorityClass ProcessPriority { get; set; }

        /// <summary>
        /// Get or set maximum execution time for running PhantomJS process (null is by default = no timeout)
        /// </summary>
        public TimeSpan? ExecutionTimeout { get; set; }

        /// <summary>
        /// Occurs when output data is received from PhantomJS process
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> OutputReceived;

        /// <summary>
        /// Occurs when error data is received from PhantomJS process
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorReceived;

        /// <summary>Create new instance of HtmlToPdfConverter</summary>
        public PhantomJS()
        {
            this.ToolPath = this.ResolveAppBinPath();
            this.PhantomJsExeName = "phantomjs.exe";
            this.ProcessPriority = ProcessPriorityClass.Normal;
        }

        private string ResolveAppBinPath()
        {
            string str = AppDomain.CurrentDomain.BaseDirectory;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType("System.Web.HttpRuntime", false);
                if (type != (Type)null)
                {
                    PropertyInfo property1 = type.GetProperty("AppDomainId", BindingFlags.Static | BindingFlags.Public);
                    if (!(property1 == (PropertyInfo)null) && property1.GetValue((object)null, (object[])null) != null)
                    {
                        PropertyInfo property2 = type.GetProperty("BinDirectory", BindingFlags.Static | BindingFlags.Public);
                        if (property2 != (PropertyInfo)null)
                        {
                            object obj = property2.GetValue((object)null, (object[])null);
                            if (obj is string)
                            {
                                str = (string)obj;
                                break;
                            }
                            break;
                        }
                        break;
                    }
                }
            }
            return str;
        }

        /// <summary>Execute javascript code from specified file.</summary>
        /// <param name="jsFile">URL or local path to javascript file to execute</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        public void Run(string jsFile, string[] jsArgs)
        {
            this.Run(jsFile, jsArgs, (Stream)null, (Stream)null);
        }

        /// <summary>
        /// Execute javascript code from specified file with input/output interaction
        /// </summary>
        /// <param name="jsFile">URL or local path to javascript file to execute</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        /// <param name="inputStream">input stream for stdin data (can be null)</param>
        /// <param name="outputStream">output stream for stdout data (can be null)</param>
        public void Run(string jsFile, string[] jsArgs, Stream inputStream, Stream outputStream)
        {
            if (jsFile == null)
                throw new ArgumentNullException("jsFile");
            this.RunInternal(jsFile, jsArgs, inputStream, outputStream);
            try
            {
                this.WaitProcessForExit();
                this.CheckExitCode(this.PhantomJsProcess.ExitCode, this.errorLines);
            }
            finally
            {
                this.PhantomJsProcess.Close();
                this.PhantomJsProcess = (Process)null;
            }
        }

        /// <summary>
        /// Asynchronously execute javascript code from specified file with input/output interaction
        /// </summary>
        /// <param name="jsFile">URL or local path to javascript file to execute</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        public Task RunAsync(string jsFile, string[] jsArgs)
        {
            if (jsFile == null)
                throw new ArgumentNullException("jsFile");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Action handleExit = (Action)(() =>
            {
                try
                {
                    this.CheckExitCode(this.PhantomJsProcess.ExitCode, this.errorLines);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    throw;
                }
                finally
                {
                    this.PhantomJsProcess.Close();
                    this.PhantomJsProcess = (Process)null;
                }
            });
            this.RunInternal(jsFile, jsArgs, (Stream)null, (Stream)null);
            this.PhantomJsProcess.Exited += (EventHandler)((sender, args) => handleExit());
            if (this.PhantomJsProcess.HasExited)
                handleExit();
            return (Task)tcs.Task;
        }

        /// <summary>Execute javascript code block</summary>
        /// <param name="javascriptCode">javascript code</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        public void RunScript(string javascriptCode, string[] jsArgs)
        {
            this.RunScript(javascriptCode, jsArgs, (Stream)null, (Stream)null);
        }

        /// <summary>Execute javascript code block</summary>
        /// <param name="javascriptCode">javascript code</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        /// <param name="inputStream">input stream for stdin data (can be null)</param>
        /// <param name="outputStream">output stream for stdout data (can be null)</param>
        public void RunScript(string javascriptCode, string[] jsArgs, Stream inputStream, Stream outputStream)
        {
            string str = Path.Combine(this.GetTempPath(), "phantomjs-" + Path.GetRandomFileName() + ".js");
            try
            {
                File.WriteAllBytes(str, Encoding.UTF8.GetBytes(javascriptCode));
                this.Run(str, jsArgs, inputStream, outputStream);
            }
            finally
            {
                this.DeleteFileIfExists(str);
            }
        }

        /// <summary>Asynchronously execute javascript code block.</summary>
        /// <param name="javascriptCode">javascript code</param>
        /// <param name="jsArgs">arguments for javascript code (optional; can be null)</param>
        public Task RunScriptAsync(string javascriptCode, string[] jsArgs)
        {
            string tmpJsFilePath = Path.Combine(this.GetTempPath(), "phantomjs-" + Path.GetRandomFileName() + ".js");
            File.WriteAllBytes(tmpJsFilePath, Encoding.UTF8.GetBytes(javascriptCode));
            try
            {
                return this.RunAsync(tmpJsFilePath, jsArgs).ContinueWith((Action<Task>)(t => this.DeleteFileIfExists(tmpJsFilePath)));
            }
            catch
            {
                this.DeleteFileIfExists(tmpJsFilePath);
                throw;
            }
        }

        private string GetTempPath()
        {
            if (!string.IsNullOrEmpty(this.TempFilesPath) && !Directory.Exists(this.TempFilesPath))
                Directory.CreateDirectory(this.TempFilesPath);
            return this.TempFilesPath ?? Path.GetTempPath();
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (filePath == null)
                return;
            if (!File.Exists(filePath))
                return;
            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }
        }

        private string PrepareCmdArg(string arg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('"');
            stringBuilder.Append(arg.Replace("\"", "\\\""));
            stringBuilder.Append('"');
            return stringBuilder.ToString();
        }

        private void RunInternal(string jsFile, string[] jsArgs, Stream inputStream, Stream outputStream)
        {
            this.errorLines.Clear();
            try
            {
                string str = Path.Combine(this.ToolPath, this.PhantomJsExeName);
                if (!File.Exists(str))
                    throw new FileNotFoundException("Cannot find PhantomJS: " + str);
                StringBuilder stringBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(this.CustomArgs))
                    stringBuilder.AppendFormat(" {0} ", (object)this.CustomArgs);
                stringBuilder.AppendFormat(" {0}", (object)this.PrepareCmdArg(jsFile));
                if (jsArgs != null)
                {
                    foreach (string jsArg in jsArgs)
                        stringBuilder.AppendFormat(" {0}", (object)this.PrepareCmdArg(jsArg));
                }
                ProcessStartInfo processStartInfo = new ProcessStartInfo(str, stringBuilder.ToString());
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(this.ToolPath);
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                this.PhantomJsProcess = new Process();
                this.PhantomJsProcess.StartInfo = processStartInfo;
                this.PhantomJsProcess.EnableRaisingEvents = true;
                this.PhantomJsProcess.Start();
                if (this.ProcessPriority != ProcessPriorityClass.Normal)
                    this.PhantomJsProcess.PriorityClass = this.ProcessPriority;
                this.PhantomJsProcess.ErrorDataReceived += (DataReceivedEventHandler)((o, args) =>
                {
                    if (args.Data == null)
                        return;
                    this.errorLines.Add(args.Data);
                    if (this.ErrorReceived == null)
                        return;
                    this.ErrorReceived((object)this, args);
                });
                this.PhantomJsProcess.BeginErrorReadLine();
                if (outputStream == null)
                {
                    this.PhantomJsProcess.OutputDataReceived += (DataReceivedEventHandler)((o, args) =>
                    {
                        if (this.OutputReceived == null)
                            return;
                        this.OutputReceived((object)this, args);
                    });
                    this.PhantomJsProcess.BeginOutputReadLine();
                }
                if (inputStream != null)
                    this.CopyToStdIn(inputStream);
                if (outputStream == null)
                    return;
                this.ReadStdOutToStream(this.PhantomJsProcess, outputStream);
            }
            catch (Exception ex)
            {
                this.EnsureProcessStopped();
                throw new Exception("Cannot execute PhantomJS: " + ex.Message, ex);
            }
        }

        protected void CopyToStdIn(Stream inputStream)
        {
            byte[] buffer = new byte[8192];
            while (true)
            {
                int count = inputStream.Read(buffer, 0, buffer.Length);
                if (count > 0)
                {
                    this.PhantomJsProcess.StandardInput.BaseStream.Write(buffer, 0, count);
                    this.PhantomJsProcess.StandardInput.BaseStream.Flush();
                }
                else
                    break;
            }
            this.PhantomJsProcess.StandardInput.Close();
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the PhantomJS standard input (stdin).
        /// </summary>
        /// <param name="s">The string to write.</param>
        /// <remarks>This method cannot be used if standard input data specified as Stream.</remarks>
        public void WriteLine(string s)
        {
            if (this.PhantomJsProcess == null)
                throw new InvalidOperationException("PhantomJS is not running");
            this.PhantomJsProcess.StandardInput.WriteLine(s);
            this.PhantomJsProcess.StandardInput.Flush();
        }

        /// <summary>Closes phantomJS process standard input stream.</summary>
        /// <remarks>This method cannot be used if standard input data specified as Stream.</remarks>
        public void WriteEnd()
        {
            this.PhantomJsProcess.StandardInput.Close();
        }

        /// <summary>Abort PhantomJS process (if started)</summary>
        /// <remarks>This method IMMEDIATELY stops PhantomJS by killing the process.</remarks>
        public void Abort()
        {
            this.EnsureProcessStopped();
        }

        private void WaitProcessForExit()
        {
            bool hasValue = this.ExecutionTimeout.HasValue;
            if (hasValue)
                this.PhantomJsProcess.WaitForExit((int)this.ExecutionTimeout.Value.TotalMilliseconds);
            else
                this.PhantomJsProcess.WaitForExit();
            if (this.PhantomJsProcess == null)
                throw new PhantomJSException(-1, "FFMpeg process was aborted");
            if (hasValue && !this.PhantomJsProcess.HasExited)
            {
                this.EnsureProcessStopped();
                throw new PhantomJSException(-2, string.Format("FFMpeg process exceeded execution timeout ({0}) and was aborted", (object)this.ExecutionTimeout));
            }
        }

        private void EnsureProcessStopped()
        {
            if (this.PhantomJsProcess == null)
                return;
            if (this.PhantomJsProcess.HasExited)
                return;
            try
            {
                this.PhantomJsProcess.Kill();
                this.PhantomJsProcess.Dispose();
                this.PhantomJsProcess = (Process)null;
            }
            catch (Exception ex)
            {
            }
        }

        private void ReadStdOutToStream(Process proc, Stream outputStream)
        {
            byte[] buffer = new byte[32768];
            int count;
            while ((count = proc.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                outputStream.Write(buffer, 0, count);
        }

        private void CheckExitCode(int exitCode, List<string> errLines)
        {
            if (exitCode != 0)
                throw new PhantomJSException(exitCode, string.Join("\n", errLines.ToArray()));
        }
    }
}

