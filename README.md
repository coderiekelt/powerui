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

* Justify

> text-align:justify

Previously PowerUI generated a box per word, which made justify relatively straight forward, but it prevented word breaking - that was challenging for Chinese. It now generates a box per line which prevents justification; a hybrid approach will be added to enable justify to work correctly again.

* Tables

> HTML tables including the caption element

Tables aren't actively being checked at the moment. The new functionality required for display:table is in place but hasn't been connected together yet.

## Testing required

These are parts of PowerUI which are currently under tested.

* The new window manager

> And it's derived systems; dialogue trees and context menus

* @Keyframes and CSS transform

> E.g. transform:rotate(90deg); inside @keyframes.

* Writing modes

> Left-to-right and vertical text

Vertical text gained basic support but hasn't been tested yet. Similarly, left-to-right hasn't been tested in PowerUI 2 yet.

* Img tag additions

> Img tag gained srcset

* The new input system

> Touch, VR and similar input modes

The input system was rebuilt, but is currently under tested. Needs to be tested out on a multitouch screen and in VR with the new [VR camera modes](http://powerui.kulestar.com/wiki/index.php?title=Virtual_Reality_Cameras).

## Down features to be worked on post release

These are all new in PowerUI 2 and have easy workarounds so they're considered minor.

* Positioning edge cases

> position:absolute;height:100%; where the ancestors height is defined by the normal flow.

These edge cases require 2+ reflow passes to handle correctly. It needs to figure out the height of the ancestor first then revisit the positioned element. PowerUI has long avoided supporting this case for this double pass reason. Slightly related is stacking contexts; the two together make reflow considerably more complex and *much* slower. However, it's very clear that these do get used in the wild, so the plan is to implement them (post MVP) in a way that it can be turned off so you can know when you're using something that's excessively slow.

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
