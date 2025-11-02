Merkle-Hellman Knapsack Cryptosystem
====================================

Overview
--------

This application is a Windows Forms-based tool developed in C# that implements the **Merkle-Hellman Knapsack Cryptosystem**, a public-key encryption algorithm based on the subset sum problem. It allows users to generate cryptographic keys, encrypt plaintext messages into ciphertext using the public key, and decrypt the ciphertext back to the original message using the private key. The application supports variable key sizes (from 8 to 4096 bits) and processes messages in blocks corresponding to the selected bit length.



Features
--------

* **Key Generation**: Generates a superincreasing private key sequence W, modulus q, and coprime multiplier r, then computes the public key B=(Wi​⋅r)modq.
* **Variable Bit Length**: Supports key sizes from 8 to 4096 bits via a dropdown selection.
* **Message Encryption**: Converts text to binary blocks, encrypts each block using the public key via subset sum.
* **Message Decryption**: Uses the private key and modular inverse of r to recover the original binary and reconstruct the plaintext.
* **Block Padding**: Automatically pads messages with spaces to align with block boundaries.
* **Error Handling**: Validates inputs and displays clear error messages for invalid operations.
* **User-Friendly Interface**: All controls are programmatically created with clear labels and multi-line text boxes for key and message display.

Usage
-----

1. **Run the Application**: Launch in a Windows environment with .NET Framework.
2. **Select Bit Length**: Choose a key size (e.g., 8, 16, ..., 4096 bits) from the dropdown.
3. **Generate Keys**:
   * Click **"Generează Chei"**.
   * The private key (W,q,r) and public key B will be displayed.
4. **Encrypt a Message**:
   * Enter plaintext in the **"Mesaj (text)"** box.
   * Click **"Criptează"**.
   * Encrypted blocks appear in **"Text Criptat"**.
5. **Decrypt the Ciphertext**:
   * Ensure the ciphertext is in the **"Text Criptat"** box.
   * Click **"Decriptează"**.
   * The original message appears in **"Mesaj Decriptat"**.

Technical Details
-----------------

* **Private Key**: Superincreasing sequence W={w1​,w2​,...,wn​} where wi+1​>∑j=1i​wj​.
* **Modulus q**: Chosen as q>∑i=1n​wi​.
* **Multiplier r**: Random integer coprime with q (i.e., gcd(r,q)=1).
* **Public Key**: Bi​=(wi​⋅r)modq.
* **Encryption**: For a binary block x∈{0,1}n, ciphertext c=∑i:xi​=1​Bi​.
* **Decryption**:
  1. Compute c′=c⋅r−1modq.
  2. Solve subset sum on superincreasing W to recover x.
  3. Reconstruct characters from binary.

Requirements
------------

* Windows OS
* .NET Framework (included with Windows)
* No external dependencies

Limitations
-----------

* **Security**: This cryptosystem is **cryptographically broken** and should **never be used** for real data protection.
* **Performance**: Large key sizes (e.g., 4096 bits) may cause delays due to BigInteger operations.
* **Character Encoding**: Uses simple 8-bit ASCII; non-ASCII characters may not decrypt correctly.


-----------------
