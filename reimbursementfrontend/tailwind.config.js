/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],
  darkMode: "media", // or 'media' or 'class'
  theme: {
    extend: {
      colors: {
        primary: "#1A1A19",
        secondary: "#FAF6E3",
        body: "#2A3663",
        light: "#859F3D",
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
};
