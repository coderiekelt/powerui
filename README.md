# PowerUI 2 (Bleeding Edge)

Hello! Welcome to the bleeding edge PowerUI 2 project. This repository doesn't currently contain a stable branch - it's simply the absolute latest commits in order to bring you the latest code as fast as possible. One does not simply make a web browser, *but let's do it anyway!*

## Down Features

Here's the list of things that are known to be not working with the reasons why.

* Example scenes

> Patchy example scene coverage

The example scenes are gradually being revisited as various features are being worked on.

* Float

> float:left in particular can display strange behaviour

Float is very difficult to get right without massively impacting the performance. It's one of the only CSS properties which can directly affect the kids of its siblings. At the moment, some test cases work and some don't.

* Scrollbars

> Automatic scrollbars aren't appearing correctly

You can manually use the new scrollbar element, but this is being worked on at the moment so it's not expected to be down for much longer.

* Justify

> text-align:justify

Previously PowerUI generated a box per word, which made justify relatively straight forward, but it prevented word breaking - that was challenging for Chinese. It now generates a box per line which prevents justification; a hybrid approach will be added to enable justify to work correctly again.

* Input elements

> Select, input and textarea

We're transitioning to using pseudo-elements to style them as browsers do. The new cursor element and select displaying is also being worked on at the moment.

* Tables

> HTML tables including the caption element

Tables aren't actively being checked at the moment. The new functionality required for display:table is in place but hasn't been connected together yet.

* SVG

> SVG imagery; not Loonim

SVG is currently a work in progress. Loonim can currently draw the paths for us but the scaling is way off.

* Woff

> The WOFF and WOFF2 font formats

PowerUI is now extremely close to supporting these. It's currently unknown what will happen if you try to load one though!

## Modules

Many of the modules present in PowerUI 2 are also present in PowerUI 1.9, however there are some important mentions:

* Nitro

> The Javascript engine

Currently, PowerUI 2 contains the old Nitro engine. This is for backwards compatibility; the first stable version will also have this engine but it will be officially depreciated.

* Wrench

> The XML engine

Wrench has been renamed and is now simply called 'Dom' as that better reflects what it actually does; it manages the DOM for a variety of different XML-structured languages (HTML, SVG, MathML, Language files).

* Loonim

> The new image generator

Loonim generates imagery using the GPU and is one of the major gems of PowerUI 2; in short, it's best experienced from its own project: https://git.kulestar.com/Loonim/UnityProject.git 
