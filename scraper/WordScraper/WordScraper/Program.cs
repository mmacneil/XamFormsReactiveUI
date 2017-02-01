

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WordScraper
{

    public class Word
    {
        public string word { get; set; }
        public string definition { get; set; }
    }

    public class Results
    {
        public int count { get; set; }
        public Word[] words { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var phantomJS = new PhantomJS.PhantomJS();
            phantomJS.OutputReceived += (sender, e) => {

                if (e.Data == null) return;

                var results = JsonConvert.DeserializeObject<Results>(e.Data);
                Console.WriteLine("PhantomJS output: {0}", e.Data);

                var csv = new StringBuilder();

                foreach (var w in results.words)
                {
                    var newLine = $"{w.word},{w.definition}";
                    csv.AppendLine(newLine);
                }

                File.WriteAllText("words.csv", csv.ToString());
            };

            phantomJS.ErrorReceived += (sender, e) => {
                Console.WriteLine("PhantomJS error: {0}", e.Data);
            };
           
                try
                {
                    phantomJS.RunScript(@"    

                    system = require('system'),
                    page = require('webpage').create()

                    var wordDetail = {
                        count: null,
                        words: []
                    }; 

                    page.open('https://www.vocabulary.com/lists/52473', function (status){
                        
                        if (status === 'success') {
							
							  if (page.injectJs('jquery.min.js')) {							  
								    var details = page.evaluate(function (wordDetail) {
                                        wordDetail.count=$('#wordlist li').length; 

                                        $('#wordlist li').each(function(index) {
                                            //console.log(index + ': ' + $(this).text());
                                            //wordDetail.words.push($(this).children(':first').text());
                                            wordDetail.words.push({
                                                word: $(this).children(':first').text(),
                                                definition: $(this).children('.definition').first().text()
                                            });
                                        });

                                       return JSON.stringify(wordDetail);
									}, wordDetail);
									
									 //console.log(title);
									system.stdout.writeLine(details);								  
							  }							
						}						

						phantom.exit();
                 });", null, null,null);
                }
                finally
                {
                    phantomJS.Abort(); // ensure that phantomjs.exe is stopped
                }

            Console.Read();
        }
    }
}
