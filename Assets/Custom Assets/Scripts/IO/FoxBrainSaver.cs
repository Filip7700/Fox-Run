using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class FoxBrainSaver {
	// Default constructor
	public FoxBrainSaver() {}

	public void SaveFoxBrainToXML(FoxBrain FoxBrainToSave, string FoxBrainFileName) {
		XmlSerializer Serializer = new XmlSerializer(typeof(FoxBrain));
		TextWriter XMLFile = new StreamWriter(FoxBrainFileName);
		Serializer.Serialize(XMLFile, FoxBrainToSave);
	}
}
