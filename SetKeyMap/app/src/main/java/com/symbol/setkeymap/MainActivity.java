package com.symbol.setkeymap;


import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.symbol.emdk.*;
import com.symbol.emdk.EMDKManager.EMDKListener;

import android.util.Xml;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;

import java.io.StringReader;

public class MainActivity extends AppCompatActivity implements EMDKListener {
    private String profileName = "defaultkeymap";
    //Declare a variable to store ProfileManager object
    private ProfileManager profileManager = null;

    //Declare a variable to store EMDKManager object
    private EMDKManager emdkManager = null;

    // Contains the parm-error name (sub-feature that has error)
    private String errorName = "";

    // Contains the characteristic-error type (Root feature that has error)
    private String errorType = "";

    // contains the error description for parm or characteristic error.
    private String errorDescription = "";

    // contains status of the profile operation
    private String status = "";


    Button rightHandBtn = null;
    Button leftHandBtn = null;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        EMDKResults results = EMDKManager.getEMDKManager(getApplicationContext(), this);

        //Check the return status of getEMDKManager
        if (results.statusCode == EMDKResults.STATUS_CODE.SUCCESS) {

        } else {
            Toast.makeText(this, "EMDKManager object creation failed", Toast.LENGTH_SHORT).show();
        }

        rightHandBtn = (Button) findViewById(R.id.righthandbutton);
        rightHandBtn.setEnabled(false);
        rightHandBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                profileName = "righthandkeymap";
                sendProfile();
            }
        });

        leftHandBtn = (Button) findViewById(R.id.lefthandbutton);
        leftHandBtn.setEnabled(false);
        leftHandBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                profileName = "lefthandkeymap";
                sendProfile();
            }
        });
    }


    @Override
    public void onClosed() {
        // TODO Auto-generated method stub
    }

    @Override
    public void onOpened(EMDKManager emdkManager) {
        this.emdkManager = emdkManager;

        profileManager = (ProfileManager) emdkManager
                .getInstance(EMDKManager.FEATURE_TYPE.PROFILE);
        if (profileManager != null) {
            rightHandBtn.setEnabled(true);
            leftHandBtn.setEnabled(true);
        }
    }

    public void sendProfile() {

        String[] modifyData = new String[1];

        EMDKResults results = profileManager.processProfile(profileName,
                ProfileManager.PROFILE_FLAG.SET, modifyData);

        if (results.statusCode == EMDKResults.STATUS_CODE.CHECK_XML) {
            String statusXMLResponse = results.getStatusString();
            // Create instance of XML Pull Parser to parse the response
            XmlPullParser parser = Xml.newPullParser();
            // Provide the string response to the String Reader that reads
            // for the parser
            try {
                parser.setInput(new StringReader(statusXMLResponse));
            } catch (XmlPullParserException e) {
                e.printStackTrace();
            }
            // Call method to parse the response
            parseXML(parser);

            if (!status.equals("Failure")) {
                Toast.makeText(this, "New keymap has been set", Toast.LENGTH_LONG).show();
            } else {
                Toast.makeText(this, "Failed to set keymap: " + errorDescription, Toast.LENGTH_LONG).show();
            }

        } else {
            Toast.makeText(this, "Failed to set KeyMap", Toast.LENGTH_SHORT).show();
        }
    }

    public void parseXML(XmlPullParser myParser) {
        int event;
        status = "";
        errorName = "";
        errorDescription = "";
        try {
            event = myParser.getEventType();
            while (event != XmlPullParser.END_DOCUMENT) {
                String name = myParser.getName();
                switch (event) {
                    case XmlPullParser.START_TAG:
                        // Get Status, error name and description in case of
                        // parm-error
                        if (name.equals("parm-error")) {
                            status = "Failure";
                            errorName = myParser.getAttributeValue(null, "name");
                            errorDescription = myParser.getAttributeValue(null,
                                    "desc");

                            // Get Status, error type and description in case of
                            // parm-error
                        } else if (name.equals("characteristic-error")) {
                            status = "Failure";
                            errorType = myParser.getAttributeValue(null, "type");
                            errorDescription = myParser.getAttributeValue(null,
                                    "desc");
                        }
                        break;
                    case XmlPullParser.END_TAG:

                        break;
                }
                event = myParser.next();

            }
        } catch (Exception e) {
            e.printStackTrace();
        }


    }

    @Override
    protected void onDestroy() {
        // TODO Auto-generated method stub
        super.onDestroy();
        //Clean up the objects created by EMDK manager
        emdkManager.release();
    }
}
