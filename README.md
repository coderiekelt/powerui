# PowerUI 2 (Bleeding Edge)

Hello! Welcome to the bleeding edge PowerUI 2 project. This repository doesn't currently contain a stable branch - it's simply the absolute latest commits in order to bring you the latest code as fast as possible. One does not simply make a web browser, *but let's do it anyway!*

## Down Features

Here's the list of things that are known to be not working with the reasons why.

* Tables

> HTML tables including the caption element

Tables are now the top priority. The new functionality required for display:table is in place but hasn't been connected together yet.

* text-shadow and text-stroke

> Both are new in PowerUI 2 but are currently incomplete

They exhibit some unstable behaviour, and text-shadow only accepts one shadow at the moment (however, the code required to allow multiple text-shadows and multiple background images is already in place).

## Testing required

These are parts of PowerUI which are currently under tested. Use at your own risk!

* The new window manager

> And its derived systems; dialogue trees and context menus

* @Keyframes and CSS transform

> E.g. transform:rotate(90deg); inside @keyframes.

@keyframes currently also needs more broad testing - particularly with the new transition functions.

* Img tag additions

> Img tag gained srcset which replaces UI.Resolution (along with CSS zoom).

* The new input system

> Touch, VR and similar input modes

The input system was rebuilt, but is currently under tested. Needs to be tested out on a multitouch screen and in VR with the new [VR camera modes](http://powerui.kulestar.com/wiki/index.php?title=Virtual_Reality_Cameras).

## Down features to be worked on post release

These are all new in PowerUI 2 and have easy workarounds so they're considered minor.

* Float edge cases

> float has some complex inheritance behaviour

* Writing modes

> Vertical text

Vertical text gained basic support but needs to be finished off.

Float is now expected to work correctly in the general case but it has complex behaviour whenever floating objects extend beyond their parent. This isn't supported just yet but will be in the future.

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
