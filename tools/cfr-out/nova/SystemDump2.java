/*
 * Decompiled with CFR 0.152.
 */
package nova;

import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Calendar;
import java.util.Vector;
import javax.swing.Box;
import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JFileChooser;
import javax.swing.JPanel;
import javax.swing.JRootPane;
import nova.Block;
import nova.Constants;
import nova.Global;
import nova.MidiInterface;
import nova.MidiMapIn;
import nova.MidiMapOut;
import nova.Nibble;
import nova.NibbleException;
import nova.NovaManager;
import nova.PrintablePanel;
import nova.Row;

public class SystemDump2
extends PrintablePanel
implements ActionListener {
    private static final int DUMP_SIZE = 526;
    private static final int DATA_START = 8;
    private static final int DATA_END = 523;
    private static final int CHECKSUM_POS = 524;
    private byte[] data;
    private String preset_name;
    private int message_id;
    private int checksum;
    private int bankN;
    private int presN;
    private int numcode;
    private File saveFile;
    private MidiMapIn mapin;
    private MidiMapOut mapout;
    static final String[] NIB_NAMES = new String[]{"#0", "RoutingType", "#2", "Vol.Min (%)", "Vol.Mid (%)", "Vol.Max (%)", "PedalType", "Master", "CC TapTempo", "CC Comp", "CC Drv", "CC Mod", "CC Delay", "CC Rev", "CC NG", "CC Pitch", "CC EQ", "CC Boost", "CC Exp.Pedal", "MIDI Channel", "PrgChange.In", "PrgChange.Out", "#22", "SysEx ID", "Midi Sync", "#25", "#26", "#27", "Tap Master", "Boost Lock", "EQ Lock", "Routing Lock", "Factory Lock", "SpeakerSim", "AngleView", "FootSwitch", "InputSrc", "Digital Clock", "#38", "FX Mute", "Dither", "TapTempo (ms)", "Digital InGain (dB)", "#43", "Adv. Mode", "Input Gain (dB)", "#46", "BoostMax (dB)", "Output Range (dBu)", "Volume (dB)", "Volume Pos", "#51", "TunerOut", "TunerRef (Hz)", "#54", "#55", "#56", "Current Preset"};
    static final int[] NIB_TYPES = new int[]{100, 3, 100, 116, 116, 116, 60, 61, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 71, 5, 5, 100, 72, 5, 100, 100, 100, 62, 5, 5, 5, 5, 5, 116, 63, 64, 65, 100, 27, 66, 123, 129, 100, 5, 112, 100, 111, 67, 102, 68, 100, 69, 130, 100, 100, 100, 7};
    private final int NIBBLE_LENGTH = 129;
    private Vector<Nibble> nibbles;
    JRootPane pane;
    Global global;
    JButton closeButton = new JButton("Close");
    JButton saveButton = new JButton("Save SysEx");
    JButton saveXML = new JButton("Export XML");
    JButton saveHTML = new JButton("Export HTML");
    JButton send = new JButton("Send to Nova");
    String title;
    Vector<Block> blocks = new Vector();
    NovaManager parent;

    public SystemDump2(NovaManager novaManager, byte[] byArray) {
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        Calendar calendar = Calendar.getInstance();
        this.title = "" + calendar.get(11);
        this.title = this.title + ":" + calendar.get(12);
        this.title = this.title + ":" + calendar.get(13);
        this.data = byArray;
        this.init();
    }

    public SystemDump2(NovaManager novaManager, String string, byte[] byArray) {
        this.parent = novaManager;
        this.setLog(novaManager.log);
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        long l = System.currentTimeMillis();
        this.title = string;
        this.data = byArray;
        this.init();
    }

    private void init() {
        JComponent jComponent;
        this.setName(this.title);
        this.nibbles = new Vector();
        try {
            for (int i = 0; i < 129; ++i) {
                byte[] byArray = new byte[4];
                for (int j = 0; j < 4; ++j) {
                    byArray[j] = this.data[8 + 4 * i + j];
                }
                jComponent = new Nibble(this, byArray, 8 + 4 * i);
                switch (i) {
                    case 0: {
                        ((Nibble)jComponent).updateNibble("#0 (1)", 100);
                        break;
                    }
                    case 58: {
                        ((Nibble)jComponent).updateNibble("#58 (512)", 100);
                        break;
                    }
                    case 59: {
                        ((Nibble)jComponent).updateNibble("#59 (1)", 100);
                        break;
                    }
                    case 60: {
                        ((Nibble)jComponent).updateNibble("#60 (15)", 100);
                        break;
                    }
                    case 62: {
                        ((Nibble)jComponent).updateNibble("#62 (810*)", 100);
                        break;
                    }
                    case 63: {
                        ((Nibble)jComponent).updateNibble("#63 (975*)", 100);
                        break;
                    }
                    case 127: {
                        ((Nibble)jComponent).updateNibble("#127 (15232)", 100);
                        break;
                    }
                    default: {
                        ((Nibble)jComponent).updateNibble("#" + i + " (0)", 100);
                    }
                }
                this.nibbles.add((Nibble)jComponent);
            }
        }
        catch (NibbleException nibbleException) {
            this.parent.log(nibbleException.getMessage());
        }
        Box box = Box.createVerticalBox();
        box.setBorder(Block.border);
        int n = 0;
        box.add(new Row("Empty", null, n++));
        box.add(new Row("2", this.nibbles.elementAt(2), n++));
        box.add(new Row("22", this.nibbles.elementAt(22), n++));
        box.add(new Row("25", this.nibbles.elementAt(25), n++));
        box.add(new Row("26", this.nibbles.elementAt(26), n++));
        box.add(new Row("27", this.nibbles.elementAt(27), n++));
        box.add(new Row("38", this.nibbles.elementAt(38), n++));
        box.add(new Row("43", this.nibbles.elementAt(43), n++));
        jComponent = Box.createVerticalBox();
        jComponent.setBorder(Block.border);
        n = 0;
        jComponent.add(new Row("Empty", null, n++));
        jComponent.add(new Row("46", this.nibbles.elementAt(46), n++));
        jComponent.add(new Row("51", this.nibbles.elementAt(51), n++));
        jComponent.add(new Row("54", this.nibbles.elementAt(54), n++));
        jComponent.add(new Row("55", this.nibbles.elementAt(55), n++));
        jComponent.add(new Row("56", this.nibbles.elementAt(56), n++));
        jComponent.add(new Row("61", this.nibbles.elementAt(61), n++));
        jComponent.add(new Row("128", this.nibbles.elementAt(128), n++));
        Box box2 = Box.createVerticalBox();
        box2.setBorder(Block.border);
        n = 0;
        box2.add(new Row("Non Empty", null, n++));
        box2.add(new Row("0", this.nibbles.elementAt(0), n++));
        box2.add(new Row("58", this.nibbles.elementAt(58), n++));
        box2.add(new Row("59", this.nibbles.elementAt(59), n++));
        box2.add(new Row("60", this.nibbles.elementAt(60), n++));
        box2.add(new Row("62", this.nibbles.elementAt(62), n++));
        box2.add(new Row("63", this.nibbles.elementAt(63), n++));
        box2.add(new Row("127", this.nibbles.elementAt(127), n++));
        Box box3 = Box.createHorizontalBox();
        box3.add(box);
        box3.add(jComponent);
        box3.add(box2);
        this.setLayout(new GridBagLayout());
        GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.insets = new Insets(0, 0, 0, 0);
        gridBagConstraints.fill = 2;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 0.0;
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        this.add((Component)box3, gridBagConstraints);
        ++gridBagConstraints.gridy;
        this.closeButton.addActionListener(this);
        this.closeButton.setActionCommand("close");
        this.saveButton.addActionListener(this);
        this.saveButton.setActionCommand("save");
        this.saveXML.addActionListener(this);
        this.saveXML.setActionCommand("saveXML");
        this.saveXML.setEnabled(true);
        this.saveHTML.addActionListener(this);
        this.saveHTML.setActionCommand("saveHTML");
        this.saveHTML.setEnabled(true);
        this.send.addActionListener(this);
        this.send.setActionCommand("send");
        this.send.setEnabled(false);
        JPanel jPanel = new JPanel();
        jPanel.setOpaque(false);
        jPanel.add(this.closeButton);
        jPanel.add(this.saveButton);
        jPanel.add(this.saveXML);
        jPanel.add(this.saveHTML);
        jPanel.add(this.send);
        ++gridBagConstraints.gridy;
        gridBagConstraints.fill = 1;
        gridBagConstraints.anchor = 20;
        gridBagConstraints.weighty = 1.0;
        this.setOpaque(true);
        this.setBackground(Constants.grisvert);
        this.add((Component)jPanel, gridBagConstraints);
    }

    int calculateChecksum() {
        return SystemDump2.calculateChecksum(this.data);
    }

    static int calculateChecksum(byte[] byArray) {
        int n = 0;
        for (int i = 8; i <= 523; ++i) {
            n += byArray[i];
        }
        return n % 128;
    }

    public void actionPerformed(ActionEvent actionEvent) {
        if (actionEvent.getActionCommand().equals("close")) {
            this.closeTab();
        } else if (actionEvent.getActionCommand().equals("save")) {
            this.saveValues();
            this.saveSysEx();
        } else if (actionEvent.getActionCommand().equals("saveXML")) {
            this.saveValues();
            this.saveXML();
        } else if (actionEvent.getActionCommand().equals("saveHTML")) {
            this.saveValues();
            this.saveHTML();
        } else if (actionEvent.getActionCommand().equals("send")) {
            this.saveValues();
            this.parent.send(this.data);
        }
    }

    public void writeBytesToFile(File file, byte[] byArray) {
        try {
            FileOutputStream fileOutputStream = new FileOutputStream(file);
            byte[] byArray2 = new byte[]{-16, 0, 32, 31, 127, 99, 69, 1, 31, 0, -9};
            fileOutputStream.write(byArray2);
            fileOutputStream.close();
            this.parent.logSilent("" + byArray2.length + " bytes written to " + file.getName());
        }
        catch (FileNotFoundException fileNotFoundException) {
            this.parent.log("FileNotFoundException : " + fileNotFoundException);
        }
        catch (IOException iOException) {
            this.parent.log("IOException : " + iOException);
        }
    }

    Nibble getNibble(int n) {
        return this.nibbles.elementAt(n);
    }

    int getNibbleCount() {
        return this.nibbles.size();
    }

    void closeTab() {
        this.parent.removeTab(this);
    }

    void saveSysEx() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileOutputStream fileOutputStream = new FileOutputStream(file);
                fileOutputStream.write(this.data);
                fileOutputStream.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveXML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toXML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveHTML() {
        JFileChooser jFileChooser = this.parent.getFileChooser();
        int n = jFileChooser.showSaveDialog(this);
        if (n == 0) {
            File file = jFileChooser.getSelectedFile();
            try {
                FileWriter fileWriter = new FileWriter(file);
                PrintWriter printWriter = new PrintWriter(fileWriter);
                printWriter.println(this.toHTML());
                printWriter.close();
            }
            catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
    }

    void saveValues() {
        for (int i = 0; i < this.nibbles.size(); ++i) {
            this.getNibble(i).save(this.data);
        }
        this.data[524] = (byte)this.calculateChecksum();
    }

    String toHTML() {
        String string = "";
        string = string + "<HTML>\n";
        string = string + "<STYLE>TABLE.MIDI TD { font-size:10px } TABLE.MIDI {table-layout: fixed}</STYLE>\n";
        string = string + "  <BODY>\n";
        string = string + "    <H1>System Dump</H1>\n";
        string = string + "    <TABLE border=\"1\" cellpadding=\"10\" width=\"1000\">\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(0).toHTML();
        string = string + this.blocks.elementAt(1).toHTML();
        string = string + this.blocks.elementAt(4).toHTML();
        string = string + this.blocks.elementAt(7).toHTML();
        string = string + this.blocks.elementAt(6).toHTML();
        string = string + "      </TR>\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(2).toHTML();
        string = string + "      </TR>\n";
        string = string + "      <TR>\n";
        string = string + this.blocks.elementAt(3).toHTML();
        string = string + this.blocks.elementAt(5).toHTML();
        string = string + this.blocks.elementAt(8).toHTML();
        string = string + "      </TR>\n";
        string = string + "    </TABLE>\n";
        string = string + this.mapin.toHTML();
        string = string + this.mapout.toHTML();
        string = string + "  </BODY>\n";
        string = string + "</HTML>\n";
        return string;
    }

    public String toXML() {
        String string = "";
        string = string + "<system vendorId=\"00201F\" vendor=\"TC Electronic\" ";
        string = string + "modelId=\"63\" model=\"Nova System\">\n";
        for (int i = 0; i < this.blocks.size(); ++i) {
            Block block = this.blocks.elementAt(i);
            string = string + block.toXML();
        }
        string = string + this.mapin.toXML();
        string = string + this.mapout.toXML();
        string = string + "</system>";
        return string;
    }

    public static boolean check(byte[] byArray) {
        boolean bl = true;
        if (bl && byArray.length != 526) {
            bl = false;
        }
        if (bl && byArray[0] != MidiInterface.aSystem[0]) {
            bl = false;
        }
        if (bl && byArray[1] != MidiInterface.aSystem[1]) {
            bl = false;
        }
        if (bl && byArray[2] != MidiInterface.aSystem[2]) {
            bl = false;
        }
        if (bl && byArray[3] != MidiInterface.aSystem[3]) {
            bl = false;
        }
        if (bl && byArray[5] != MidiInterface.aSystem[5]) {
            bl = false;
        }
        if (bl && byArray[6] != MidiInterface.aSystem[6]) {
            bl = false;
        }
        if (bl && byArray[7] != MidiInterface.aSystem[7]) {
            bl = false;
        }
        if (bl && byArray[525] != -9) {
            bl = false;
        }
        if (bl && byArray[524] != SystemDump2.calculateChecksum(byArray)) {
            bl = false;
        }
        return bl;
    }

    void setStandalone(boolean bl) {
        this.send.setEnabled(!bl);
    }

    public static void main(String[] stringArray) {
        if (stringArray.length != 1) {
            System.out.println("Syntax : java SystemDump <Sysex inputFile>");
        } else {
            File file = new File(stringArray[0]);
            try {
                byte[] byArray = NovaManager.getBytesFromFile(file);
                NovaManager novaManager = new NovaManager();
                novaManager.setEditable(false);
                SystemDump2 systemDump2 = new SystemDump2(novaManager, stringArray[0], byArray);
                novaManager.add(systemDump2);
            }
            catch (IOException iOException) {
                System.err.println("File not found : " + stringArray[1]);
            }
        }
    }
}
