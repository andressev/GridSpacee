This is a repository for a unity project to explore the parameter space of discrete celullar automata. Right now if you download the project and run it, or simply run the build you will have acces to a neighborhood editor, wich will give you the option to add your own neighborhoods to the automata. 



## What is an automata?

Take a grid of pixels, imagine that for each of those pixels you can run a function at the same time as all other pixels. Now John Conway thought, I will ask each pixel/cell how many of its neighbors that are directly next to it are alive (turned on), if the cell is alive and has 2-3 alive neighbors it will live, if its dead and has exactly 3 live neighbors it will come back to life.

Conway's Nieghborhood:

![Conway](/mdIMages/ConwayNH.png)


This little system is the famous Conway's game of life. As impressive as it is, conways game of life is one of the simplest 2D Cellullar Automata,  there is no limit for what the rules can be or even the neighborhoods they can be anything and as many as your computer is able to process. Conway's game of life is a single blip in a (maybe) infinite parameter space. 



## What is GridSpace?

This gamified tool is a way to explore the parameter space of cellular automata, inspired heavily by Slackermanz who discoverd and implemented the multiple neighborhood cellular automata (MNCA) variation and has been exploring it for the past decade. With Slackermanz help and insight I built GridSpace, where you can create neighborhoods and add them to the automata, to explore the neighborhoods and their rules. 
To explore the rules a mutation system is implemented where you can press tab to get four new random rules, or press space to generate mutated variants out of one of the rules. 
You can also mutate the whole thing in mutation mode, where you can generete new random neighborhoods.


## Downloading the build
To download the build download the file int the [Build](Build) folder.