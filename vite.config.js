import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';

export default defineConfig({
  clearScreen: false,
  server: {
    watch: {
        ignored: [
            "**/*.md" , // Don't watch markdown files
            "**/*.fs" , // Don't watch F# files
            "**/*.fsx"  // Don't watch F# script files
        ]
    }
  },

  plugins: [
    solidPlugin()
  ],
  base: "./" // This is important for the app to work correctly when deployed to GitHub Pages to start al paths with a ./
});