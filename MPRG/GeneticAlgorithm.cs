using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class GenericAlgorithm
    {
        public List<AiOpponent> population;
        public List<AiOpponent> alivePopulation;
        int genSize = 10;
        public Texture2D texture;

        public bool waitingForNewGen = false;

        public GenericAlgorithm(Texture2D texture2d)
        {
            texture = texture2d;
        }

        float[] inherited(float[] parent1, float[] parent2)
        {
            float[] child = new float[parent1.Length];
            for (int i = 0; i < parent1.Length; i++)
            {
                child[i] = (parent1[i] + parent2[i]) / 2;
            }
            return child;
        }

        public void newGen()
        {
            population.Sort((a, b) => b.score.CompareTo(a.score));
            var best = population.Take(5).ToList();
            var children = new List<AiOpponent>(best);

            Random random = new Random();
            while (children.Count < genSize)
            {
                var parent1 = best[random.Next(best.Count)].DNA;
                var parent2 = best[random.Next(best.Count)].DNA;

                float[] childGene = inherited(parent1, parent2);
                Mutation(childGene);

                children.Add(new AiOpponent(texture, new Vector2(640, 390), childGene));
            }
            population = children;
            waitingForNewGen = false;
        }

        public void Mutation(float[] DNA)
        {
            for (int i = 0; i < DNA.Length; i++)
            {
                if (Random.Shared.Next(0, 100) < 10)
                {
                    DNA[i] += (float)Random.Shared.NextDouble() * (0.1f - -0.1f) - 0.1f;
                }
            }
        }

        public void saveData(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, "DNA.txt");
            using StreamWriter streamWriter = new StreamWriter(filePath);

            foreach (var ai in population)
            {
                streamWriter.WriteLine(string.Join(",", ai.DNA));
            }
        }

        public void loadData(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine("data Found");
                population = new List<AiOpponent>();
                string filePath = Path.Combine(path, "DNA.txt");
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    float[] genes = line.Split(',').Select(float.Parse).ToArray();
                    population.Add(new AiOpponent(texture, new Vector2(640, 390), genes));
                }
            }
            else
            {
                Console.WriteLine("No data Found");
                population = new List<AiOpponent>();
                for (int i = 0; i < genSize; i++)
                {
                    population.Add(new AiOpponent(texture, new Vector2(640, 390)));
                }
            }
            
            alivePopulation = new List<AiOpponent>();
            foreach (AiOpponent ai in population)
            {
                alivePopulation.Add(ai);
            }
            population.AsReadOnly();
        }
    }
}