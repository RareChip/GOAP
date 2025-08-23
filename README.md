# GOAP
A Goal-Oriented Action Planning AI framework made for Unity development and ease of access with Scriptable Objects.

!! This GitHub page is still under **major development** !!

While GOAP implementations for Unity exist, I wanted to make my own to further advance my understanding in the algorithm and to provide features some of the other implementations didnt provide.

* Scriptable Objects
  * Everything about this implementation uses Scriptable Objects, making modifications and adjustments extremely simple.
* Flexible Conditions
  * Conditions can use all logical operators.
* Configuration
  * Every GOAP-Agent has a mandatory configuration Scriptable Object that initialized the Agent's knowledge about the world. This neatly groups all sensor logic into one place.
 
*Eventually, I will add a guide to this page explaining how to use all features.*

# WARNING

```DefaultActionConfig.cs``` uses ```[SerializeReference]``` to serialize an interface. Unity does not support this by default, so unless you have Odin inspector or a custom editor for interface serialization, you will not be able to use the default action config. In a complete version of this asset, I anticipate some work-around for this.

