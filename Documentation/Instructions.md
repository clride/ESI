# Instructions

The concept of the language is based entirely on manipulating memory. The program is given an array of integers, often referred to as cells, which can be manipulated to allow for custom behaviour.

`ESR` allows all basic `Brainf***` instructions except for the `.` instruction, which is would be used to output characters to the Console.

These include:

- `+`
  
  - Adds 1 to the current cell value

- `-`
  
  - Subtracts 1 from the current cell value

- `>`
  
  - Selects the next cell

- `<`
  
  - Selects the previous cell

- `,`
  
  - Read input from the Console

- `[ ... ]`
  
  - Represents a loop
  
  - A loop is only exited if the value of the selected cell becomes 0.



However, in this implementation there are some new instructions.

But first, let's cover the 2 types of memory ESR allows you to manipulate:

- `Normal memory`
  
  - An integer array that can contain any content.

- `Framebuffer memory`
  
  - An integer array containing integers which represent characters
  
  - These characters can also use 16 different colors
    
    - How? Well, let's look at how characters are represented normally:
      
      A = 65 in ASCII.
      
      ConsoleColor provides 16 different colors, white being Color 15.
      
      To encode `A` as a white character, you would multiply the size of the ASCII instruction set by the Color Value, and then add the ASCII index of `A`.
      
      => 65 + 127 * 15 = 1970.
      
      This is the value you would set the cell to to achieve a white `A` letter.

Now, the new instructions:

- `$ ... $`
  
  - Any code written between 2 dollar signs will be executed in the context of `Framebuffer memory`, which means when you edit a cell, it will display directly to the Console. This means that when you set a cell to 1, and then write code between two `$`, it will show as 0, since this is not the same memory.

- `;`
  
  - This instruction refreshes the Console, ensuring it displays the up-to date `framebuffer`.

- `?`
  
  - This instruction sets the value of the current cell to the address of the current cell.

- `|`
  
  - This instruction forms the bridge between `Normal memory` and `Framebuffer memory`. It sets the value of the current cell to the value the same cell in the other memory array has.
  
  - Example: if you set address 0 of the `Normal memory` to 1, and then start editing the `framebuffer`, you would have to call `|` to set address 0 of the `framebuffer` to 1 aswell.


