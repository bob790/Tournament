using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Tournament
{
    public class Tourney
    {
        public List<Competitor> allCompetitors;
        public List<Competitor> activeCompetitors;
        public List<Match> history;
        public Match currentMatch;
        public int round;
        public int remainder;
        public string saveFile;

        public Tourney(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }
            activeCompetitors = new List<Competitor>();
            allCompetitors = new List<Competitor>();
            history = new List<Match>();
            string ext = Path.GetExtension(filename);
            if (ext == ".xml")
            {
                String name1 = null;
                String name2 = null;
                int epi = 4;
                using (XmlReader rd = XmlReader.Create(filename))
                {
                    while (rd.Read())
                    {
                            if (rd.Name == "Match")
                            {
                                    rd.Read();
                                    name1 = rd.ReadElementString("Player1");
                                    name2 = rd.ReadElementString("Player2");
                                    epi = Convert.ToInt32(rd.ReadElementString("Episode"));
                                    round = Convert.ToInt32(rd.ReadElementString("Round"));
                                    remainder = Convert.ToInt32(rd.ReadElementString("Remainder"));
                                    String str = rd.ReadOuterXml();
                                    while (str != null & str!="")
                                    {
                                        XmlReader reader = XmlReader.Create(new StringReader(str));
                                        while (reader.Read())
                                        {
                                            if (reader.Name == "Competitor")
                                            {
                                                reader.Read();
                                                String name = reader.ReadElementString("title");
                                                int reach = Convert.ToInt32(reader.ReadElementString("round"));
                                                bool elim = Convert.ToBoolean(reader.ReadElementString("elim"));
                                                Competitor cp = new Competitor(name, reach, elim);
                                                allCompetitors.Add(cp);
                                                cp.round = reach;
                                            }
                                            if (reader.Name == "History")
                                            {
                                                reader.Read();
                                                String p1 = reader.ReadElementString("player1");
                                                String p2 = reader.ReadElementString("player2");
                                                int esipode = Convert.ToInt32(reader.ReadElementString("episode"));
                                                int rond = Convert.ToInt32(reader.ReadElementString("round"));
                                                Competitor c1 = new Competitor();
                                                Competitor c2 = new Competitor();
                                                foreach (Competitor c in allCompetitors)
                                                {
                                                    if (c.name == p1)
                                                    {
                                                        c1 = c;
                                                    }
                                                    if (c.name == p2)
                                                    {
                                                        c2 = c;
                                                    }
                                                }
                                                Match temp = new Match(c1, c2, esipode);
                                                temp.round = rond;
                                                history.Add(temp);
                                            }
                                        }
                                        str = rd.ReadOuterXml();
                                    }
                            }

                    }
                    saveFile = filename;
                    Competitor cp1 = new Competitor();
                    Competitor cp2 = new Competitor();
                    foreach (Competitor cp in allCompetitors)
                    {
                        if (cp.name == name1)
                        {
                            cp1 = cp;
                        }
                        if (cp.name == name2)
                        {
                            cp2 = cp;
                        }
                        if (cp.round >= round)
                        {
                            activeCompetitors.Add(cp);
                        }
                    }
                    Random rnd = new Random();
                    activeCompetitors = activeCompetitors.OrderBy(x => rnd.Next()).ToList();
                    if (!String.IsNullOrEmpty(cp1.name) && !String.IsNullOrEmpty(cp2.name))
                    {
                        currentMatch = new Match(cp1, cp2, epi);
                        currentMatch.round = round;
                    }
                    else
                    {
                        this.nextMatch();
                    }
                }
            }
            else
            {
                try
                {
                    using (StreamReader rd = new StreamReader(filename))
                    {
                        string name;
                        while ((name = rd.ReadLine()) != null)
                        {
                            Competitor cp = new Competitor(name);
                            allCompetitors.Add(cp);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
                round = 1;
                saveFile = String.Concat(Path.GetFileNameWithoutExtension(filename), ".xml");
                Random rnd = new Random();
                activeCompetitors = allCompetitors.OrderBy(x => rnd.Next()).ToList();

                int num = 1;
                while (num < activeCompetitors.Count)
                {
                    num = num * 2;
                }
                num = num / 2;
                int req = activeCompetitors.Count - num;
                if (req != 0)
                {
                    req = (activeCompetitors.Count - (req * 2));
                    for (int i = 0; i < req; i++)
                    {
                        activeCompetitors[i].round++;
                    }
                }
                this.nextMatch();
                remainder = activeCompetitors.Count;
            }
        }

        public void nextMatch()
        {
            if (activeCompetitors.Count == 1) 
            {
                currentMatch = null;
                currentMatch = new Match(activeCompetitors[0], null);
                return; 
            }
            int i = 0;
            Competitor c1 = new Competitor();
            Competitor c2 = new Competitor();
            while (i < activeCompetitors.Count)
            {
                if (activeCompetitors[i].round == round & !activeCompetitors[i].eliminated)
                {
                    if (c1.name == null)
                    {
                        c1 = activeCompetitors[i];
                    }
                    else
                    {
                        c2 = activeCompetitors[i];
                        break;
                    }
                }
                i++;
            }
            if ((String.IsNullOrEmpty(c1.name) | String.IsNullOrEmpty(c2.name)) && activeCompetitors.Count > 1)
            {
                this.nextRound();
                this.nextMatch();
                return;
            }
            else
            {
                if (activeCompetitors.Count < 2)
                {
                    currentMatch = null;
                    currentMatch = new Match(c1, c2);
                    currentMatch.round = round;
                    return;
                }
            }
            try
            {
                if (!String.IsNullOrEmpty(currentMatch.player2.name))
                {
                    history.Add(currentMatch);
                }
            }
            catch (Exception e)
            {
            }
            currentMatch = null;
            currentMatch = new Match(c1, c2);
            currentMatch.round = round;
        }

        public void nextRound()
        {
            List<Competitor> nextRoundCompetitors = new List<Competitor>();
            foreach (Competitor cp in activeCompetitors)
            {
                if (cp.round > round)
                {
                    nextRoundCompetitors.Add(cp);
                }
                else
                {
                    if (!cp.eliminated)
                    {
                        throw new Exception("Tried to start a new round for no reason");
                    }
                }
            }
            activeCompetitors.Clear();
            Random rnd = new Random();
            activeCompetitors = nextRoundCompetitors.OrderBy(x => rnd.Next()).ToList();
            round++;
            remainder = activeCompetitors.Count;
        }

        public void saveGame()
        {
            if(currentMatch.player2==null)
            {
                return;
            }
            using (XmlWriter writer = XmlWriter.Create(saveFile))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Match");
                writer.WriteElementString("Player1", currentMatch.player1.name);
                writer.WriteElementString("Player2", currentMatch.player2.name);
                writer.WriteElementString("Episode", currentMatch.episode.ToString());
                writer.WriteElementString("Round", round.ToString());
                writer.WriteElementString("Remainder", remainder.ToString());
                foreach (Competitor cp in allCompetitors)
                {
                    writer.WriteStartElement("Competitor");
                    writer.WriteElementString("title", cp.name);
                    writer.WriteElementString("round", cp.round.ToString());
                    writer.WriteElementString("elim", cp.eliminated.ToString());
                    writer.WriteEndElement();
                }
                foreach(Match m in history)
                {
                    writer.WriteStartElement("History");
                    writer.WriteElementString("player1", m.player1.name);
                    writer.WriteElementString("player2", m.player2.name);
                    writer.WriteElementString("episode", m.episode.ToString());
                    writer.WriteElementString("round", m.round.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }

    public class Match 
    {
        public Competitor player1;
        public Competitor player2;
        public int round;
        private int Episode;
        public int episode
        {
            get
            {
                return Episode;
            }
            set
            {
                if (value < 11 & value > 1)
                {
                    Episode = value;
                }
                else
                {
                    Episode = 4;
                }
            }
        }

        public Match(Competitor c1, Competitor c2)
        {
            player1 = c1;
            player2 = c2;
            Random rand = new Random();
            episode = rand.Next(2, 11);
        }

        public Match(Competitor c1, Competitor c2, int epi)
        {
            player1 = c1;
            player2 = c2;
            episode = epi;
        }

        public void Winner(int input)
        {
            if (input == 1)
            {
                player1.round++;
                player2.eliminated = true;
            }
            else
            {
                if (input == 2)
                {
                    player2.round++;
                    player1.eliminated = true;
                }
            }
        }
    }

    public class Competitor
    {
        private string Name;
        public string name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        public bool eliminated;
        private int Round;
        public int round
        {
            get
            {
                return Round;
            }
            set
            {
                Round = value;
            }
        }

        public Competitor()
        {
            name = null;
        }

        public Competitor(string input)
        {
            name = input;
            eliminated = false;
            round = 1;
        }

        public Competitor(string input, int rd, bool elim)
        {
            name = input;
            eliminated = elim;
            round = rd;
        }
    }

}
