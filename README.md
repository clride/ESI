# ESR-Interpreter

An interpreter for a custom esoteric programming language called **ES**.  
The interpreter is referred to as **ESRI** (*Esoteric Software Runtime Interpreter*) or **ESI** for short.

---

## About the Language

**ES** is based on [Brainf***](https://en.wikipedia.org/wiki/Brainfuck), an esoteric language created by Urban Müller in 1993 with only eight instructions but still Turing complete. 

I extended the instruction set to support both a normal memory array and a framebuffer for simple console graphics. The goal was not practicality, but to explore language design, memory management, and writing an interpreter.

To see how to use it and how it differs from the original Brainf***, look at the [Documentation](./Documentation) directory.

---

**Environment**

This project was created using `.NET 8.0`.
