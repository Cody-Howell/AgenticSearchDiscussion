export default function Home() {
  return (
    <div className="max-w-5xl mx-auto space-y-6">
      <div className="bg-emerald-950/60 rounded-2xl border border-emerald-900/70 shadow-lg shadow-emerald-950/40 p-8 backdrop-blur-sm">
        <h1 className="text-4xl font-bold text-emerald-50 mb-4 pb-2 border-b border-emerald-800">Home</h1>
        <p className="text-emerald-100/90 text-lg mb-6">
          This app provides a few features to help with brainstorming and
          coding.
        </p>
        
        <h2 className="text-2xl font-semibold text-emerald-50 mt-8 mb-3 pl-3 border-l-4 border-emerald-600">
          Planning
        </h2>
        <p className="text-emerald-100/80 mb-4">
          Using the Todo section, go and input a large multi-step prompt into
          the "Break" box. This will be sent to an LLM to break down into a
          smaller amount of steps for another LLM to come in and discuss all of
          those points. An example has been provided below.
        </p>
        <p className="text-emerald-50 italic bg-emerald-900/60 p-4 rounded-xl border border-emerald-800 shadow-inner shadow-emerald-950/50">
          Make a plan for a tower defense game. Design 5 tower types, describing their capabilities, 
          and 5 enemies, describing their attributes and weaknesses. Think through either a static 
          map design or a way to generate fun maps for the player. Come up with other things to 
          collect and do outside the primary gameplay loop. Design a progression system. 
        </p>
        
        <h2 className="text-2xl font-semibold text-emerald-50 mt-8 mb-3 pl-3 border-l-4 border-emerald-600">
          Reading Code
        </h2>
        <p className="text-emerald-100/80">
          You can see the some local codebases I've cloned. You can explore files in the
          File tab. The project is the first select and the file is the second select.
        </p>
        <p className="text-emerald-100/80">
          In chats, the LLM can query the repositories and read files. It gets a table of contents of every file in one 
          repository, then makes as many tool calls as necessary to anwer your question. From my experience, it 
          usually gets the README, then any other files it thinks are relevant. 
        </p>
      </div>
    </div>
  );
}
