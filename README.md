# BarcodeTestImageGenerator
Co-op Project: Crops barcode snippets from images and pastes them on to new images and applies any desired filters to generate a set of new test images for barcode readers

This is just a program I created during my second co-op. Barcode readers were under development and need large sets of test images to stress test the readers.

This made it easier by making "new" test images from existing ones by cropping the barcodes out as snippets and stitching them onto other test images to created hundreds of new combinations.

<b>A variety of additional settings can make the endless new combinations including:</b>
* Set or randomize barcode
  - Placement
  - Rotation angle
  - Skew angle
* Set or randomize filters on the entire image:
  - Blur
  - Noise
  - Contrast
  
<b>The amount of new test images can be changed</b>
  * One-to-one: creates a new test image for every existing base image (i.e. put a new barcode on every test image)
  * All possible combinations: creates a new test image for every combination of existing barcode snippets + base image
  * Specific amount: Randomly stitches combinations of barcode snippet + base image until the specified amount is reached
  
<b>Speed</b>

The program can crop out 40-45 barcode snippets per second and can stitch 45-50 barcode snippets onto base images per second.
Stitching (creating new images) is slightly faster because it uses multithreading. Cropping was already fast enough that it didn't need multithreading.
  
